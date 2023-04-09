using LiteNetLib;
using LiteNetLib.Utils;
using Mono.Nat;
using Nebula.Net.Nat;
using Nebula.Net.Packets.Responses;
using Serilog;

// ReSharper disable InconsistentNaming

namespace Nebula.Net.Services.Server;

public class NetServerService : NetListener, INetServerService
{
    private readonly INatMapperService _natMapperService;
    protected readonly Dictionary<int, ClientPeer> _connectedClients = new();

    public NetServerService(ILogger logger, INatMapperService natMapperService) : base(logger, nameof(NetServerService))
    {
        _natMapperService = natMapperService;
    }

    private NatMapping? NatMapping { get; set; }

    public async Task Start(NetOptions netOptions)
    {
        if (IsRunning || !netOptions.IsValidForServer)
            return;

        _connectedClients.Clear();

        if (netOptions.UseUpnp)
        {
            NatMapping = await _natMapperService.Map(Protocol.Udp, netOptions.ServerPort, "NebulaParty");
            if (NatMapping == null)
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
        if (NatMapping != null)
        {
            await _natMapperService.Unmap(NatMapping);
            NatMapping = null;
        }

        NetOptions = null;
        _logger.Information("Server stopped !");
    }

    public void SendPacket<TPacket>(ref TPacket packet, NetPeer user, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where TPacket : INetSerializable, new()
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

    protected void BroadcastClientsList()
    {
        var connectedClients = _connectedClients.Values.Select(p => new ClientInfo { Id = (uint)p.Peer.Id, Username = p.Username }).ToArray();
        var clientListPacket = new ClientsListPacket { Clients = connectedClients };
        _netPacketProcessor.Write(_netDataWriter, clientListPacket);
        _netManager.SendToAll(_netDataWriter, DeliveryMethod.ReliableOrdered);
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

                BroadcastClientsList();
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
        BroadcastClientsList();
        _logger.Information("Peer {EndPoint} disconnected", $"{peer.EndPoint.Address}:{peer.EndPoint.Port}");
    }
}