using LiteNetLib.Utils;

namespace Nebula.Net.Packets;

public interface IEmptyNetSerializable : INetSerializable
{
    void INetSerializable.Serialize(NetDataWriter writer)
    {
        const byte b = 1;
        writer.Put(b);
    }

    void INetSerializable.Deserialize(NetDataReader reader)
    {
        _ = reader.GetByte();
    }
}
