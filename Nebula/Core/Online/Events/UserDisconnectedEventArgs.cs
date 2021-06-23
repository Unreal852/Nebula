using System;
using Nebula.Net.Data;

namespace Nebula.Core.Online.Events
{
    public class UserDisconnectedEventArgs : EventArgs
    {
        public UserDisconnectedEventArgs(NetUserInfo userInfo)
        {
            UserInfo = userInfo;
        }

        public NetUserInfo UserInfo { get; }
    }
}