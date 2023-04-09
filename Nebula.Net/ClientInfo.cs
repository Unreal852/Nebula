using LiteNetLib.Utils;

namespace Nebula.Net;

public struct ClientInfo : INetSerializable
{
    public uint Id { get; set; }
    public string Username { get; set; }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Id);
        writer.Put(Username);
    }

    public void Deserialize(NetDataReader reader)
    {
        Id = reader.GetUInt();
        Username = reader.GetString();
    }
}
