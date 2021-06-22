using Nebula.Net.Data;

namespace Nebula.Net.Packet
{
    public class UserMessagePacket
    {
        public NetUserInfo Sender  { get; set; }
        public NetMessage  Message { get; set; }
    }
}