using LiteNetLib;
using LiteNetLib.Utils;

namespace Nebula.Net.Services;

public interface INetService
{
    NetOptions? NetOptions { get; }
    bool IsRunning { get; }

    void SendPacket<TPacket>(ref TPacket packet, NetPeer user, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where TPacket : INetSerializable, new();
}