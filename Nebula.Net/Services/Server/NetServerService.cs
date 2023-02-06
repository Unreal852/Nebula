using LiteNetLib;
using LiteNetLib.Utils;
using Mono.Nat;
using Nebula.Net.Nat;
using Serilog;

// ReSharper disable InconsistentNaming

namespace Nebula.Net.Services.Server;

public class NetServerService : NetListener, INetServerService
{
    protected readonly Dictionary<int, ClientPeer> ConnectedClients = new();

    public NetServerService(ILogger logger) : base(logger, nameof(NetServerService))
    {
    }

    private Mapping?    UpnpMapping { get; set; }
    private INatDevice? NatDevice   { get; set; }

    public void SubscribePacket<TPacket, TUserData>(Action<TPacket, TUserData> packetHandler)
            where TPacket : INetSerializable, new()
    {
        NetPacketProcessor.SubscribeNetSerializable(packetHandler);
    }

    public void UnsubscribePacketHandler<TPacket>() where TPacket : INetSerializable, new()
    {
        NetPacketProcessor.RemoveSubscription<TPacket>();
    }

    public override async Task Start()
    {
        if (!CanStart)
            return;
        ConnectedClients.Clear();

        if (NetOptions!.UseUpnp) await CreateUpnpMapping();

        NetManager.Start(NetOptions!.ServerPort);
        Logger.Information("Server started ! {@NetOptions}", NetOptions);
    }

    public override async Task Stop()
    {
        if (!IsRunning)
            return;
        NetManager.Stop(true);
        await DeleteUpnpMapping();
        Logger.Information("Server stopped !");
    }

    public void SendPacket<TPacket>(ref TPacket packet, NetPeer user,
                                    DeliveryMethod method = DeliveryMethod.ReliableOrdered)
            where TPacket : INetSerializable, new()
    {
        if (!IsRunning)
            return;
        NetPacketProcessor.SendNetSerializable(user, ref packet, method);
    }

    public void BroadcastPacket<TPacket>(ref TPacket packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered)
            where TPacket : INetSerializable, new()
    {
        if (!IsRunning)
            return;
        NetPacketProcessor.SendNetSerializable(NetManager, ref packet, method);
    }

    private async Task CreateUpnpMapping()
    {
        using var discoverer = new NatDiscoverer();
        var cancellationSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        NatDevice = await discoverer.DiscoverDeviceAsync(cancellationSource.Token);
        if (NatDevice == null)
        {
            Logger.Warning("No device found, upnp discovery failed");
            return;
        }

        var mapping = new Mapping(Protocol.Udp, NetOptions!.ServerPort, NetOptions!.ServerPort, 0, "NebulaServer");
        UpnpMapping = await NatDevice.CreatePortMapAsync(mapping);
        if (UpnpMapping == null)
            Logger.Warning("Failed to create upnp mapping");
        else
            Logger.Information("Successfully mapped upnp port");
    }

    private async Task DeleteUpnpMapping()
    {
        if (NatDevice == null || UpnpMapping == null)
            return;
        await NatDevice.DeletePortMapAsync(UpnpMapping);
        NatDevice = null;
        UpnpMapping = null;
    }

    public override void OnConnectionRequest(ConnectionRequest request)
    {
        if (NetManager.ConnectedPeersCount >= NetOptions!.ServerSlots)
        {
            request.Reject(NetDataWriter.FromString("Server full"));
            return;
        }

        if (NetOptions!.HasPassword)
            request.Accept();
        else
            request.AcceptIfKey(NetOptions!.ServerPassword);
    }

    public override void OnPeerConnected(NetPeer peer)
    {
        ConnectedClients.Add(peer.Id, new ClientPeer(peer));
        Logger.Information("Peer connected from {EndPoint}", peer.EndPoint);
    }

    public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        ConnectedClients.Remove(peer.Id);
        Logger.Information("Peer {EndPoint} disconnected", $"{peer.EndPoint.Address}:{peer.EndPoint.Port}");
    }
}