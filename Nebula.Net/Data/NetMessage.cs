using LiteNetLib.Utils;

namespace Nebula.Net.Data
{
    public struct NetMessage : INetSerializable
    {
        public string Message { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Message);
        }

        public void Deserialize(NetDataReader reader)
        {
            Message = reader.GetString();
        }
    }
}