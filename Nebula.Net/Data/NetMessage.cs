using LiteNetLib.Utils;

namespace Nebula.Net.Data
{
    public struct NetMessage : INetSerializable
    {
        public string Message     { get; set; }
        public int    MessageType { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Message);
            writer.Put(MessageType);
        }

        public void Deserialize(NetDataReader reader)
        {
            Message = reader.GetString();
            MessageType = reader.GetInt();
        }
    }
}