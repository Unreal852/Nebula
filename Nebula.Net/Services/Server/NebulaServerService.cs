using LiteNetLib;
using Nebula.Common.Audio;
using Nebula.Net.Packets.Requests;
using Nebula.Net.Packets.Responses;
using Serilog;

namespace Nebula.Net.Services.Server;

public sealed class NebulaServerService : NetServerService
{
    public NebulaServerService() : base(Log.Logger)
    {
        SubscribePacket<YoutubeMusicRequestPacket, NetPeer>(OnReceiveYoutubeMusicRequest);
        SubscribePacket<PlayerPlayRequestPacket, NetPeer>(OnReceivePlayerPlayRequest);
        SubscribePacket<PlayerPauseRequestPacket, NetPeer>(OnReceivePlayerPauseRequest);
        SubscribePacket<PlayerPositionRequestPacket, NetPeer>(OnReceivePlayerPositionRequest);
        SubscribePacket<ClientReadyRequestPacket, NetPeer>(OnReceiveClientReadyRequest);
    }

    public  AudioServiceState PlayerState    { get; set; }
    private bool              IsOpeningMedia { get; set; }

    private void OnReceiveYoutubeMusicRequest(YoutubeMusicRequestPacket packet, NetPeer peer)
    {
        if (IsOpeningMedia) // Don't open a new media if we are already opening a media
            return;
        IsOpeningMedia = true;

        foreach (KeyValuePair<int, ClientPeer> connectedClient in ConnectedClients)
            connectedClient.Value.IsReady = false;

        var responsePacket = new YoutubeMusicResponsePacket { VideoId = packet.VideoId };
        BroadcastPacket(ref responsePacket);
    }

    private void OnReceivePlayerPlayRequest(PlayerPlayRequestPacket packet, NetPeer peer)
    {
        if (IsOpeningMedia) // Don't send the play packet if a media is being opened
            return;
        var responsePacket = new PlayerPlayResponsePacket();
        BroadcastPacket(ref responsePacket);
    }

    private void OnReceivePlayerPauseRequest(PlayerPauseRequestPacket packet, NetPeer peer)
    {
        if (IsOpeningMedia) // Same as play
            return;
        var responsePacket = new PlayerPauseResponsePacket();
        BroadcastPacket(ref responsePacket);
    }

    private void OnReceivePlayerPositionRequest(PlayerPositionRequestPacket packet, NetPeer peer)
    {
        if (IsOpeningMedia) // Same as play
            return;
        var responsePacket = new PlayerPositionResponsePacket { Position = packet.Position };
        BroadcastPacket(ref responsePacket);
    }

    private void OnReceiveClientReadyRequest(ClientReadyRequestPacket packet, NetPeer peer)
    {
        if (!IsOpeningMedia) // Ignore, this should not happen
        {
            Logger.Warning("Received a {PacketType} but we are not opening any media",
                    nameof(ClientReadyRequestPacket));
            return;
        }

        if (ConnectedClients.TryGetValue(peer.Id, out ClientPeer? value))
            value.IsReady = true;

        foreach (KeyValuePair<int, ClientPeer> connectedClient in ConnectedClients)
            if (!connectedClient.Value.IsReady)
                return;

        var responsePacket = new PlayerPlayResponsePacket();
        BroadcastPacket(ref responsePacket);

        IsOpeningMedia = false;
    }
}