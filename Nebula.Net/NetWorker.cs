using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Nebula.Net.Data;

namespace Nebula.Net
{
    public class NetWorker : INetEventListener
    {
        public NetWorker()
        {
            NetManager = new NetManager(this) {UnsyncedEvents = true, AutoRecycle = true};
            NetProcessor = new NetPacketProcessor();
            NetProcessor.RegisterNestedType<NetMediaInfo>();
            NetProcessor.RegisterNestedType<NetSessionInfo>();
            NetProcessor.RegisterNestedType<NetUserInfo>();
            NetProcessor.RegisterNestedType<NetMessage>();
        }

        public NetManager         NetManager   { get; }
        public NetPacketProcessor NetProcessor { get; }

        public void SendPacket<T>(T packet, NetPeer user, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            NetProcessor.Send(user, packet, method);
        }

        public virtual void OnConnectionRequest(ConnectionRequest request)
        {
        }

        public virtual void OnPeerConnected(NetPeer peer)
        {
        }

        public virtual void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
        }

        public virtual void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
        }

        public virtual void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            NetProcessor.ReadAllPackets(reader);
        }

        public virtual void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }

        public virtual void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }
    }
}