using LiteNetLib;
using Nebula.Net.Data;

namespace Nebula.Net.Server
{
    public class NetServerClient
    {
        public NetServerClient(NetPeer peer)
        {
            Peer = peer;
        }

        public NetPeer     Peer           { get; }
        public NetUserInfo UserInfo       { get; set; }
        public bool        HasDefaultInfo { get; set; }
        public bool        IsPlayReady    { get; set; }
        public int         BadPackets     { get; set; }
        public int         Id             => Peer?.Id ?? -1;
    }
}