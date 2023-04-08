using LiteNetLib.Utils;

namespace Nebula.Net.Packets.Responses;

public struct ClientConnectedPacket : INetSerializable
{
    public uint Id;
    public string Username;

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
