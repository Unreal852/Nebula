using LiteNetLib;

namespace Nebula.Net.Services.Server;

public sealed class ClientPeer
{
    public ClientPeer(NetPeer peer)
    {
        Peer = peer;
    }

    public NetPeer Peer    { get; }
    public bool    IsReady { get; set; }
}