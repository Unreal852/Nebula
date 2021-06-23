using Nebula.Net.Data;

namespace Nebula.Net.Packet
{
    public class PlayerOpenMediaPacket
    {
        public NetUserInfo  Sender { get; set; }
        public NetMediaInfo Media  { get; set; }
    }
}