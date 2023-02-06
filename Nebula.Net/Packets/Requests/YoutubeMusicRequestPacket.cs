using LiteNetLib.Utils;

namespace Nebula.Net.Packets.Requests;

public struct YoutubeMusicRequestPacket : INetSerializable
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