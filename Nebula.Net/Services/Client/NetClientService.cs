using LiteNetLib;
using LiteNetLib.Utils;
using Serilog;

namespace Nebula.Net.Services.Client;

public sealed class NetClientService : NetListener, INetClientService
{
    public NetClientService(ILogger logger) : base(logger, nameof(NetClientService))
    {
    }

    private NetPeer? ServerPeer { get; set; }

    public event EventHandler<EventArgs>? Connecting;
    public event EventHandler<NetPeer>? Connected;
    public event EventHandler<NetPeer?>? Disconnected;

    public void Connect(NetOptions netOptions, string clientUsername)
    {
        if (IsRunning || !netOptions.IsValidForClient)
            return;

        Connecting?.Invoke(this, EventArgs.Empty);
        _logger.Information("Connecting to server... {@NetOptions}", netOptions);
        if (_netManager.Start())
        {
            var netPeer = _netManager.Connect(netOptions.ServerAddress, netOptions.ServerPort, NetDataWriter.FromString(clientUsername));
            if (netPeer != null)
                NetOptions = netOptions;
        }
        else
            _logger.Error("Failed to start net manager ! {@NetOptions}", netOptions);
    }

    public void Disconnect()
    {
        if (!IsRunning)
            return;
        _netManager.Stop(true);
        NetOptions = null;
        Disconnected?.Invoke(this, null);
    }

    public void SubscribeNetPacket<TPacket>(Action<TPacket> packetHandler) where TPacket : INetSerializable, new()
    {
        _netPacketProcessor.SubscribeNetSerializable(packetHandler);
    }

    public void SubscribePacket<TPacket>(Action<TPacket> packetHandler) where TPacket : class, new()
    {
        _netPacketProcessor.SubscribeReusable(packetHandler);
    }

    public void SendPacket<TPacket>(ref TPacket packet, NetPeer user, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where TPacket : INetSerializable, new()
    {
        if (!IsRunning)
            return;
        _netPacketProcessor.WriteNetSerializable(_netDataWriter, ref packet);
        ServerPeer!.Send(_netDataWriter, method);
        _netDataWriter.Reset();
    }

    public void SendPacket<TPacket>(ref TPacket packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where TPacket : INetSerializable, new()
    {
        SendPacket(ref packet, ServerPeer!, method);
    }

    public override void OnPeerConnected(NetPeer peer)
    {
        ServerPeer = peer;
        if (ServerPeer != null)
        {
            _logger.Information("Connected to server");
            Connected?.Invoke(this, peer);
        }
    }

    public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        ServerPeer = null;
        _logger.Information("Disconnected from server | Reason: {Reason} | Data: {Data} | SocketError: {SocketError} ",
                disconnectInfo.Reason,
                disconnectInfo.AdditionalData.EndOfData ? "NULL" : disconnectInfo.AdditionalData.GetString(),
                disconnectInfo.SocketErrorCode);
        Disconnected?.Invoke(this, peer);
    }
}