using LiteNetLib.Utils;

namespace Nebula.Net.Data
{
    public struct NetUserInfo : INetSerializable
    {
        public int    Id        { get; private set; }
        public string Username  { get; set; }
        public string AvatarUrl { get; set; }

        public NetUserInfo WithId(int id)
        {
            return new() {Id = id, Username = Username, AvatarUrl = AvatarUrl};
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put(Username);
            writer.Put(AvatarUrl);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetInt();
            Username = reader.GetString();
            AvatarUrl = reader.GetString();
        }
    }
}