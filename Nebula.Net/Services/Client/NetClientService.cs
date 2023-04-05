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
    public event EventHandler<NetPeer>? Disconnected;

    public override Task Start()
    {
        if (!CanStart)
            return Task.CompletedTask;
        NetManager.Start();
        Logger.Information("Connecting to server... Options: {@Options}", NetOptions);
        Connecting?.Invoke(this, EventArgs.Empty);
        NetManager.Connect(NetOptions!.ServerAddress, NetOptions.ServerPort, NetOptions.ServerPassword);
        return Task.CompletedTask;
    }

    public override Task Stop()
    {
        if (!IsRunning)
            return Task.CompletedTask;
        NetManager.Stop(true);
        return Task.CompletedTask;
    }

    public void SubscribePacket<TPacket>(Action<TPacket> packetHandler) where TPacket : INetSerializable, new()
    {
        NetPacketProcessor.SubscribeNetSerializable(packetHandler);
    }

    public void UnsubscribePacketHandler<TPacket>() where TPacket : INetSerializable, new()
    {
        NetPacketProcessor.RemoveSubscription<TPacket>();
    }

    public void SendPacket<TPacket>(ref TPacket packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered)
            where TPacket : INetSerializable, new()
    {
        if (!IsRunning)
            return;
        NetPacketProcessor.WriteNetSerializable(NetDataWriter, ref packet);
        ServerPeer!.Send(NetDataWriter, method);
    }

    public override void OnPeerConnected(NetPeer peer)
    {
        ServerPeer = peer;
        if (ServerPeer != null)
        {
            Logger.Information("Connected to server");
            Connected?.Invoke(this, peer);
        }
    }

    public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        ServerPeer = null;
        Logger.Information("Disconnected from server | Reason: {Reason} | Data: {Data} | SocketError: {SocketError} ",
                disconnectInfo.Reason,
                disconnectInfo.AdditionalData.EndOfData ? "NULL" : disconnectInfo.AdditionalData.GetString(),
                disconnectInfo.SocketErrorCode);
        Disconnected?.Invoke(this, peer);
    }
}