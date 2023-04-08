using LiteNetLib.Utils;

namespace Nebula.Net.Packets.Responses;

public struct ClientDisconnectedPacket : INetSerializable
{
    public uint Id;

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Id);
    }

    public void Deserialize(NetDataReader reader)
    {
        Id = reader.GetUInt();
    }
}
