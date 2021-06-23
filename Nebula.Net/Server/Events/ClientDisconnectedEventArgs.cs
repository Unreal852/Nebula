using System;

namespace Nebula.Net.Server.Events
{
    public class ClientDisconnectedEventArgs : EventArgs
    {
        public ClientDisconnectedEventArgs(NetServerClient serverClient)
        {
            ServerClient = serverClient;
        }

        public NetServerClient ServerClient { get; }
    }
}