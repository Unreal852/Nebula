﻿using LiteNetLib;
using LiteNetLib.Utils;

namespace Nebula.Net.Services.Client;

public interface INetClientService : INetService
{
    public event EventHandler<EventArgs>? Connecting;
    public event EventHandler<NetPeer>? Connected;
    public event EventHandler<NetPeer?>? Disconnected;

    void Connect(NetOptions netOptions, string clientUsername);
    void Disconnect();
    void SubscribeNetPacket<TPacket>(Action<TPacket> packetHandler) where TPacket : INetSerializable, new();
    void SubscribePacket<TPacket>(Action<TPacket> packetHandler) where TPacket : class, new();
    void SendPacket<TPacket>(ref TPacket packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where TPacket : INetSerializable, new();
}