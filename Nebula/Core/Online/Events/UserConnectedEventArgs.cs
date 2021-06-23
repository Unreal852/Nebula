using System;
using Nebula.Net.Data;

namespace Nebula.Core.Online.Events
{
    public class UserConnectedEventArgs : EventArgs
    {
        public UserConnectedEventArgs(NetUserInfo userInfo)
        {
            UserInfo = userInfo;
        }

        public NetUserInfo UserInfo { get; }
    }
}