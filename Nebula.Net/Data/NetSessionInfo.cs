using LiteNetLib.Utils;

namespace Nebula.Net.Data
{
    public struct NetSessionInfo : INetSerializable
    {
        public string SessionName  { get; set; }
        public int    ClientsCount { get; set; }
        public int    MaxClients   { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(SessionName);
            writer.Put(ClientsCount);
            writer.Put(MaxClients);
        }

        public void Deserialize(NetDataReader reader)
        {
            SessionName = reader.GetString();
            ClientsCount = reader.GetInt();
            MaxClients = reader.GetInt();
        }
    }
}