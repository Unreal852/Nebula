using Nebula.Net.Data;

namespace Nebula.Net.Packet.S2C
{
    public class SessionInfoPacket
    {
        public NetSessionInfo SessionInfo { get; set; }
        public NetUserInfo[]  Users       { get; set; }
    }
}