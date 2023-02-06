using LiteNetLib.Utils;

namespace Nebula.Net.Packets.Responses;

public struct YoutubeMusicResponsePacket : INetSerializable
{
    public string VideoId;

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(VideoId);
    }

    public void Deserialize(NetDataReader reader)
    {
        VideoId = reader.GetString();
    }
}