using LiteNetLib.Utils;

namespace Nebula.Net.Services;

public interface INetService
{
    NetOptions? NetOptions { get; set; }
    bool        IsRunning  { get; }

    Task Start();
    Task Stop();
    void UnsubscribePacketHandler<TPacket>() where TPacket : INetSerializable, new();
}