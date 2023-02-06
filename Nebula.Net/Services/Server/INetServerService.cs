using LiteNetLib;
using LiteNetLib.Utils;

namespace Nebula.Net.Services.Server;

public interface INetServerService : INetService
{
    void SubscribePacket<TPacket, TUserData>(Action<TPacket, TUserData> packetHandler)
            where TPacket : INetSerializable, new();

    void SendPacket<TPacket>(ref TPacket packet, NetPeer user, DeliveryMethod method = DeliveryMethod.ReliableOrdered)
            where TPacket : INetSerializable, new();

    void BroadcastPacket<TPacket>(ref TPacket packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered)
            where TPacket : INetSerializable, new();
}