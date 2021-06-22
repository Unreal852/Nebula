using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using Nebula.Net.Data;
using Nebula.Net.Packet;
using Nebula.Net.Packet.S2C;
using Open.Nat;

namespace Nebula.Net.Server
{
    public class NetServer : NetWorker
    {
        public NetServer(NetServerSettings settings)
        {
            Settings = settings ?? NetServerSettings.Default;
            NetProcessor.SubscribeReusable<UserInfoPacket, NetServerClient>(OnReceiveUserInfos);
            NetProcessor.SubscribeReusable<UserMessagePacket, NetServerClient>(OnReceiveUserMessage);
        }

        public  NetServerSettings                Settings          { get; }
        public  NatDevice                        NatDevice         { get; private set; }
        private Dictionary<int, NetServerClient> Clients           { get; } = new();
        private NetMediaInfo                     CurrentMedia      { get; set; }
        private NetSessionInfo                   SessionInfo       { get; set; }
        private TimeSpan                         LastMediaChange   { get; set; } = TimeSpan.Zero;
        private TimeSpan                         LastSessionChange { get; set; } = TimeSpan.Zero;

        public async Task<bool> Start()
        {
            if (Settings.UseUpNp)
            {
                var discoverer = new NatDiscoverer();
                var cts = new CancellationTokenSource(Settings.UpNpTimeOut);
                NatDevice = await discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts);
                if (NatDevice == null)
                    return false;
                await NatDevice.CreatePortMapAsync(new Mapping(Protocol.Udp, Settings.ServerPort, Settings.ServerPort, "NebulaServer"));
            }

            return NetManager.Start(Settings.ServerPort);
        }

        public void Stop()
        {
            NetManager.Stop(true);
            Clients.Clear();
            CurrentMedia = default;
            LastMediaChange = TimeSpan.Zero;
        }

        public void BroadcastPacket<T>(T packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            NetProcessor.Send(NetManager, packet, method);
        }

        public NetServerClient GetClient(int id)
        {
            return Clients.ContainsKey(id) ? Clients[id] : null;
        }

        public void HandleBadPacket(NetServerClient client)
        {
            client.BadPackets++;
            if (client.BadPackets >= Settings.BadPacketsLimit)
                client.Peer.Disconnect(NetDataWriter.FromString($"Too many bad packets ({client.BadPackets})"));
        }

        public override void OnConnectionRequest(ConnectionRequest request)
        {
            if (NetManager.ConnectedPeersCount >= Settings.MaxClients)
            {
                request.Reject(NetDataWriter.FromString("Server Full"));
                return;
            }

            if (!string.IsNullOrWhiteSpace(Settings.Key))
                request.AcceptIfKey(Settings.Key);
            else
                request.Accept();
        }

        public override void OnPeerConnected(NetPeer peer)
        {
            if (Clients.ContainsKey(peer.Id))
            {
                peer.Disconnect(NetDataWriter.FromString("Someone with the same id is already connected."));
                return;
            }

            Clients.Add(peer.Id, new NetServerClient(peer));
        }

        public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            NetServerClient serverClient = GetClient(peer.Id);
            if (serverClient == null)
                return;
            BroadcastPacket(new UserDisconnectedPacket {User = serverClient.UserInfo});
        }

        public override void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            NetServerClient client = GetClient(peer.Id);
            if (client == null)
            {
                peer.Disconnect(NetDataWriter.FromString("Client not registered server side"));
                return;
            }

            try
            {
                NetProcessor.ReadAllPackets(reader, client);
            }
            catch
            {
                HandleBadPacket(client);
            }
        }

        private void OnReceiveUserInfos(UserInfoPacket packet, NetServerClient client)
        {
            client.UserInfo = packet.UserInfo.WithId(client.Id);
            BroadcastPacket(new UserInfoPacket {UserInfo = client.UserInfo});
        }

        private void OnReceiveUserMessage(UserMessagePacket packet, NetServerClient client)
        {
            BroadcastPacket(new UserMessagePacket {Sender = client.UserInfo, Message = packet.Message});
        }
    }
}