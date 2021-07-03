using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using Nebula.Net.Data;
using Nebula.Net.Packet;
using Nebula.Net.Packet.C2S;
using Nebula.Net.Packet.S2C;
using Nebula.Net.Server.Events;
using Open.Nat;

namespace Nebula.Net.Server
{
    public class NetServer : NetWorker
    {
        private NetMediaInfo _currentMedia;

        public NetServer()
        {
            NetProcessor.SubscribeReusable<UserInfoPacket, NetServerClient>(OnReceiveUserInfos);
            NetProcessor.SubscribeReusable<UserMessagePacket, NetServerClient>(OnReceiveUserMessage);
            NetProcessor.SubscribeReusable<PlayerOpenMediaPacket, NetServerClient>(OnReceiveOpenMedia);
            NetProcessor.SubscribeReusable<PlayerReadyPacket, NetServerClient>(OnReceivePlayReady);
            NetProcessor.SubscribeReusable<PlayerPausePacket, NetServerClient>(OnReceivePlayerPause);
            NetProcessor.SubscribeReusable<PlayerResumePacket, NetServerClient>(OnReceivePlayerResume);
            NetProcessor.SubscribeReusable<PlayerPositionPacket, NetServerClient>(OnReceivePlayerPosition);
        }

        public  NetServerSettings                Settings          { get; private set; }
        public  NatDevice                        NatDevice         { get; private set; }
        private Dictionary<int, NetServerClient> Clients           { get; } = new();
        private int                              Id                { get; set; }
        private DateTime                         LastMediaChange   { get; set; } = DateTime.UtcNow;
        private TimeSpan                         LastSessionChange { get; set; } = TimeSpan.Zero;

        private NetMediaInfo CurrentMedia
        {
            get => _currentMedia;
            set
            {
                _currentMedia = value;
                LastMediaChange = DateTime.UtcNow;
            }
        }

        public event EventHandler<ClientConnectedEventArgs>    ClientConnected;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;

        public async Task<bool> Start(NetServerSettings settings = null)
        {
            Settings = settings ?? NetServerSettings.Default;
            Id = new Random().Next(1000, 100000);
            if (Settings.UseUpnp)
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

        public async Task Stop()
        {
            NetManager.Stop(true);
            await NatDevice.DeletePortMapAsync(await NatDevice.GetSpecificMappingAsync(Protocol.Udp, Settings.ServerPort));
            NatDevice = null;
            Clients.Clear();
            CurrentMedia = default;
            LastMediaChange = DateTime.Now;
            LastSessionChange = TimeSpan.Zero;
        }

        public void SetAllNotReady()
        {
            foreach (var kvp in Clients)
                kvp.Value.IsPlayReady = false;
        }

        public bool AreAllReady()
        {
            foreach (var kvp in Clients)
            {
                if (!kvp.Value.IsPlayReady)
                    return false;
            }

            return true;
        }

        public SessionInfoPacket GetSessionInfoPacket()
        {
            SessionInfoPacket packet = new SessionInfoPacket
            {
                SessionInfo = new NetSessionInfo {Id = Id, ClientsCount = NetManager.ConnectedPeersCount, MaxClients = Settings.MaxClients},
                Users = Clients.Where(kvp => kvp.Value.HasDefaultInfo).Select(kvp => kvp.Value.UserInfo).ToArray()
            };
            return packet;
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

            var client = new NetServerClient(peer);
            Clients.Add(peer.Id, client);
            SendPacket(GetSessionInfoPacket(), peer);
            ClientConnected?.Invoke(this, new ClientConnectedEventArgs(client));
        }

        public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            NetServerClient serverClient = GetClient(peer.Id);
            if (serverClient == null)
                return;
            BroadcastPacket(new UserDisconnectedPacket {User = serverClient.UserInfo});
            BroadcastPacket(GetSessionInfoPacket());
            ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(serverClient));
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
            if (!client.HasDefaultInfo)
            {
                client.HasDefaultInfo = true;
                BroadcastPacket(new UserConnectedPacket {User = client.UserInfo});
            }
            else
                BroadcastPacket(new UserInfoPacket {UserInfo = client.UserInfo});
            BroadcastPacket(GetSessionInfoPacket());
        }

        private void OnReceiveUserMessage(UserMessagePacket packet, NetServerClient client)
        {
            BroadcastPacket(new UserMessagePacket {Sender = client.UserInfo, Message = packet.Message});
        }

        private void OnReceivePlayerPause(PlayerPausePacket packet, NetServerClient client)
        {
            BroadcastPacket(packet);
        }

        private void OnReceivePlayerResume(PlayerResumePacket packet, NetServerClient client)
        {
            BroadcastPacket(packet);
        }

        private void OnReceivePlayerPosition(PlayerPositionPacket packet, NetServerClient client)
        {
            BroadcastPacket(packet);
        }

        private void OnReceiveOpenMedia(PlayerOpenMediaPacket packet, NetServerClient client)
        {
            if ((DateTime.UtcNow - LastMediaChange).TotalMilliseconds < Settings.MediaChangeDelay)
                return;
            SetAllNotReady();
            packet.Sender = client.UserInfo;
            BroadcastPacket(packet);
        }

        private void OnReceivePlayReady(PlayerReadyPacket packet, NetServerClient client)
        {
            client.IsPlayReady = packet.IsReady;
            if (AreAllReady())
                BroadcastPacket(new PlayerPlayPacket());
        }
    }
}