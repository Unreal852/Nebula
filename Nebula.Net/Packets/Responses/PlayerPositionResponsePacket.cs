using LiteNetLib.Utils;

namespace Nebula.Net.Packets.Responses;

public struct PlayerPositionResponsePacket : INetSerializable
{
    public double Position;

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Position);
    }

    public void Deserialize(NetDataReader reader)
    {
        Position = reader.GetDouble();
    }
}