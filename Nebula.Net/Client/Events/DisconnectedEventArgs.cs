using System;
using LiteNetLib;

namespace Nebula.Net.Client.Events
{
    public class DisconnectedEventArgs : EventArgs
    {
        public DisconnectedEventArgs(DisconnectInfo disconnectInfo)
        {
            Info = disconnectInfo;
        }

        public DisconnectInfo Info { get; }
    }
}