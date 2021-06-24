using System;
using LiteNetLib;
using Nebula.Net.Client.Events;

namespace Nebula.Net.Client
{
    public class NetClient : NetWorker
    {
        public NetClient()
        {
        }

        public NetPeer ServerPeer  { get; private set; }
        public bool    IsConnected => ServerPeer != null && IsRunning;

        public event EventHandler<ConnectedEventArgs>    Connected;
        public event EventHandler<DisconnectedEventArgs> Disconnected;

        public void Connect(string ip, int port, string key = "")
        {
            if (IsConnected)
                return;
            NetManager.Start();
            NetManager.Connect(ip, port, key);
        }

        public void Disconnect()
        {
            if (!IsConnected)
                return;
            NetManager.Stop(true);
        }

        public void SendPacket<T>(T packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            SendPacket(packet, ServerPeer, method);
        }

        public override void OnPeerConnected(NetPeer peer)
        {
            ServerPeer = peer;
            Connected?.Invoke(this, new ConnectedEventArgs());
        }

        public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            ServerPeer = null;
            Disconnected?.Invoke(this, new DisconnectedEventArgs(disconnectInfo));
        }
    }
}