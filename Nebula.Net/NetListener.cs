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
    protected readonly ILogger _logger;
    protected readonly NetManager _netManager;
    protected readonly NetDataWriter _netDataWriter;
    protected readonly NetPacketProcessor _netPacketProcessor;
    private NetOptions? _netOptions;

    protected NetListener(ILogger logger, string? loggerContext = null)
    {
        _logger = logger.ForContext("ClassContext", loggerContext ?? nameof(NetListener));
        _netManager = new NetManager(this) { UnsyncedEvents = true, AutoRecycle = true };
        _netDataWriter = new NetDataWriter();
        _netPacketProcessor = new NetPacketProcessor();

        _netPacketProcessor.RegisterNestedType<YoutubeMusicRequestPacket>();
        _netPacketProcessor.RegisterNestedType<YoutubeMusicResponsePacket>();
        _netPacketProcessor.RegisterNestedType<PlayerPlayRequestPacket>();
        _netPacketProcessor.RegisterNestedType<PlayerPlayResponsePacket>();
        _netPacketProcessor.RegisterNestedType<PlayerPauseRequestPacket>();
        _netPacketProcessor.RegisterNestedType<PlayerPauseResponsePacket>();
        _netPacketProcessor.RegisterNestedType<PlayerPositionRequestPacket>();
        _netPacketProcessor.RegisterNestedType<PlayerPositionResponsePacket>();
        _netPacketProcessor.RegisterNestedType<ClientReadyRequestPacket>();
    }

    public bool IsRunning => _netManager.IsRunning;

    public NetOptions? NetOptions
    {
        get => _netOptions;
        set
        {
            //if (IsRunning)
            //{
            //    _logger.Warning("Network options can't be changed if the network manager is running");
            //    return;
            //}

            _netOptions = value;
        }
    }
    public void SubscribePacket<TPacket, TUserData>(Action<TPacket, TUserData> packetHandler) where TPacket : INetSerializable, new()
    {
        _netPacketProcessor.SubscribeNetSerializable(packetHandler);
    }

    public void UnsubscribePacketHandler<TPacket>() where TPacket : INetSerializable, new()
    {
        _netPacketProcessor.RemoveSubscription<TPacket>();
    }

    public virtual void OnPeerConnected(NetPeer peer)
    {
    }

    public virtual void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
    }

    public virtual void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        _logger.Warning("Network error '{SocketError}' from {EndPoint}", socketError, endPoint);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        try
        {
            _netPacketProcessor.ReadAllPackets(reader, peer);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Network receive error");
        }
    }

    public virtual void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
    }

    public virtual void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public virtual void OnConnectionRequest(ConnectionRequest request)
    {
    }
}