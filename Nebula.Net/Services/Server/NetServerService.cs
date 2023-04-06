using LiteNetLib;
using LiteNetLib.Utils;
using Mono.Nat;
using Nebula.Net.Nat;
using Serilog;

// ReSharper disable InconsistentNaming

namespace Nebula.Net.Services.Server;

public class NetServerService : NetListener, INetServerService
{
    protected readonly Dictionary<int, ClientPeer> _connectedClients = new();

    public NetServerService(ILogger logger) : base(logger, nameof(NetServerService))
    {
    }

    private Mapping? UpnpMapping { get; set; }
    private INatDevice? NatDevice { get; set; }

    public async Task Start(NetOptions netOptions)
    {
        if (IsRunning || !netOptions.IsValidForServer)
            return;

        _connectedClients.Clear();

        if (netOptions.UseUpnp)
        {
            var upnpMappingResult = await CreateUpnpMapping(netOptions.ServerPort);
            if (!upnpMappingResult)
                return;
        }

        if (_netManager.Start(netOptions.ServerPort))
        {
            NetOptions = netOptions;
            _logger.Information("Server started ! {@NetOptions}", netOptions);
        }
        else
            _logger.Error("Failed to start server ! {@NetOptions}", netOptions);

    }

    public async Task Stop()
    {
        if (!IsRunning)
            return;
        _netManager.Stop(true);
        await DeleteUpnpMapping();
        NetOptions = null;
        _logger.Information("Server stopped !");
    }

    private async Task<bool> CreateUpnpMapping(int port)
    {
        using var discoverer = new NatDiscoverer();
        var cancellationSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        NatDevice = await discoverer.DiscoverDeviceAsync(cancellationSource.Token);
        if (NatDevice == null)
        {
            _logger.Warning("No device found, upnp discovery failed");
            return false;
        }

        var mapping = new Mapping(Protocol.Udp, port, port, 0, "NebulaServer");
        UpnpMapping = await NatDevice.CreatePortMapAsync(mapping);
        if (UpnpMapping == null)
        {
            _logger.Warning("Failed to create upnp mapping");
            return false;
        }

        _logger.Information("Successfully mapped upnp port");
        return true;
    }

    private async Task DeleteUpnpMapping()
    {
        if (NatDevice == null || UpnpMapping == null)
            return;
        await NatDevice.DeletePortMapAsync(UpnpMapping);
        NatDevice = null;
        UpnpMapping = null;
    }

    public void SendPacket<TPacket>(ref TPacket packet, NetPeer user,
                                    DeliveryMethod method = DeliveryMethod.ReliableOrdered) where TPacket : INetSerializable, new()
    {
        if (!IsRunning)
            return;
        _netPacketProcessor.WriteNetSerializable(_netDataWriter, ref packet);
        user.Send(_netDataWriter, method);
        _netDataWriter.Reset();
    }

    public void BroadcastPacket<TPacket>(ref TPacket packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where TPacket : INetSerializable, new()
    {
        if (!IsRunning)
            return;
        _netPacketProcessor.WriteNetSerializable(_netDataWriter, ref packet);
        _netManager.SendToAll(_netDataWriter, method);
        _netDataWriter.Reset();
    }

    public override void OnConnectionRequest(ConnectionRequest request)
    {
        if (_netManager.ConnectedPeersCount >= NetOptions!.ServerSlots)
        {
            request.Reject(NetDataWriter.FromString("Server full"));
            return;
        }

        if (request.Data.TryGetString(out var username))
        {
            var netPeer = request.Accept();
            if (netPeer != null)
            {
                var clientPeer = new ClientPeer(netPeer, username);
                _connectedClients.Add(netPeer.Id, clientPeer);
            }
        }
        else
            request.Reject(NetDataWriter.FromString("Missing username"));
    }

    public override void OnPeerConnected(NetPeer peer)
    {
        _logger.Information("Peer {EndPoint} connected", peer.EndPoint);
    }

    public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        _connectedClients.Remove(peer.Id);
        _logger.Information("Peer {EndPoint} disconnected", $"{peer.EndPoint.Address}:{peer.EndPoint.Port}");
    }
}