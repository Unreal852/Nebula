using LiteNetLib.Utils;

namespace Nebula.Net.Packets.Responses;

public class ClientsListPacket
{
    public ClientInfo[] Clients { get; set; }
}
