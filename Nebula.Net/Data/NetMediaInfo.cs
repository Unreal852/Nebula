using LiteNetLib.Utils;

namespace Nebula.Net.Data
{
    public struct NetMediaInfo : INetSerializable
    {
        public string Id     { get; set; }
        public string Title  { get; set; }
        public string Author { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put(Title);
            writer.Put(Author);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetString();
            Title = reader.GetString();
            Author = reader.GetString();
        }
    }
}