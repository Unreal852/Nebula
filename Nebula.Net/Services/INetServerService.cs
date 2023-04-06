using LiteNetLib;
using LiteNetLib.Utils;

namespace Nebula.Net.Services;

public interface INetServerService : INetService
{
    Task Start(NetOptions netOptions);
    Task Stop();
    void BroadcastPacket<TPacket>(ref TPacket packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where TPacket : INetSerializable, new();
}