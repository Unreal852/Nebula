using LiteNetLib.Utils;

namespace Nebula.Net.Data
{
    public struct NetSessionInfo : INetSerializable
    {
        public static NetSessionInfo Default { get; } = new() {Id = -1, ClientsCount = -1, MaxClients = -1};

        public int Id           { get; set; }
        public int ClientsCount { get; set; }
        public int MaxClients   { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put(ClientsCount);
            writer.Put(MaxClients);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetInt();
            ClientsCount = reader.GetInt();
            MaxClients = reader.GetInt();
        }
    }
}