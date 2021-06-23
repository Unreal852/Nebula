using System;

namespace Nebula.Net.Server.Events
{
    public class ClientConnectedEventArgs : EventArgs
    {
        public ClientConnectedEventArgs(NetServerClient serverClient)
        {
            ServerClient = serverClient;
        }

        public NetServerClient ServerClient { get; }
    }
}