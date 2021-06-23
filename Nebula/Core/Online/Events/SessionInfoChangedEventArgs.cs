using System;
using Nebula.Net.Data;

namespace Nebula.Core.Online.Events
{
    public class SessionInfoChangedEventArgs : EventArgs
    {
        public SessionInfoChangedEventArgs(NetSessionInfo sessionInfo, NetUserInfo[] users)
        {
            SessionInfo = sessionInfo;
            Users = users;
        }

        public NetSessionInfo SessionInfo { get; }
        public NetUserInfo[]  Users       { get; }
    }
}