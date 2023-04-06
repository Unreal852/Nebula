using LiteNetLib;

namespace Nebula.Net.Services.Server;

public sealed class ClientPeer
{
    public ClientPeer(NetPeer peer)
    {
        Peer = peer;
        Username = peer.Tag as string ?? $"UnknownUser{Random.Shared.Next(1000, 10000)}";
    }

    public NetPeer Peer { get; }
    public string Username { get; }
    public bool IsReady { get; set; }
}