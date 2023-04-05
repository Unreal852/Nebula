using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Nebula.Net.Packets.Requests;
using Nebula.Net.Packets.Responses;
using Serilog;

namespace Nebula.Net;

public abstract class NetListener : INetEventListener
{
    protected readonly ILogger Logger;
    private NetOptions? _netOptions;

    protected NetListener(ILogger logger, string? loggerContext = null)
    {
        Logger = logger.ForContext("ClassContext", loggerContext ?? nameof(NetListener));
        NetManager = new NetManager(this) { UnsyncedEvents = true, AutoRecycle = true };
        NetDataWriter = new NetDataWriter();
        NetPacketProcessor = new NetPacketProcessor();

        NetPacketProcessor.RegisterNestedType<YoutubeMusicRequestPacket>();
        NetPacketProcessor.RegisterNestedType<YoutubeMusicResponsePacket>();
        NetPacketProcessor.RegisterNestedType<PlayerPlayRequestPacket>();
        NetPacketProcessor.RegisterNestedType<PlayerPlayResponsePacket>();
        NetPacketProcessor.RegisterNestedType<PlayerPauseRequestPacket>();
        NetPacketProcessor.RegisterNestedType<PlayerPauseResponsePacket>();
        NetPacketProcessor.RegisterNestedType<PlayerPositionRequestPacket>();
        NetPacketProcessor.RegisterNestedType<PlayerPositionResponsePacket>();
        NetPacketProcessor.RegisterNestedType<ClientReadyRequestPacket>();
    }

    public NetManager NetManager { get; }
    public NetDataWriter NetDataWriter { get; }
    public NetPacketProcessor NetPacketProcessor { get; }
    public bool IsRunning => NetManager.IsRunning;
    public bool CanStart => !IsRunning && NetOptions != null;

    public NetOptions? NetOptions
    {
        get => _netOptions;
        set
        {
            if (IsRunning)
            {
                Logger.Warning("Network options can't be changed if the network manager is running");
                return;
            }

            _netOptions = value;
        }
    }

    public virtual void OnPeerConnected(NetPeer peer)
    {
    }

    public virtual void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
    }

    public virtual void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        Logger.Warning("Network error from {EndPoint} | {SocketError}", endPoint, socketError.ToString());
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber,
                                 DeliveryMethod deliveryMethod)
    {
        try
        {
            NetPacketProcessor.ReadAllPackets(reader, peer);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Network receive error");
        }
    }

    public virtual void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
                                                    UnconnectedMessageType messageType)
    {
    }

    public virtual void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public virtual void OnConnectionRequest(ConnectionRequest request)
    {
    }

    public abstract Task Start();
    public abstract Task Stop();
}