using LiteNetLib.Utils;

namespace Nebula.Net;

public class ClientInfo : INetSerializable
{
    public uint Id { get; set; }
    public string Username { get; set; }

    public void Deserialize(NetDataReader reader)
    {
        Id = reader.GetUInt();
        Username = reader.GetString();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Id);
        writer.Put(Username);
    }
}
