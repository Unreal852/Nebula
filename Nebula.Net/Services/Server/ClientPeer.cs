using LiteNetLib;

namespace Nebula.Net.Services.Server;

public sealed class ClientPeer
{
    public ClientPeer(NetPeer peer, string username)
    {
        Peer = peer;
        Username = username;
    }

    public NetPeer Peer { get; }
    public string Username { get; }
    public bool IsReady { get; set; }
}