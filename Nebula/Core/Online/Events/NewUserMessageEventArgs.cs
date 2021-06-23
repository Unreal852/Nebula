using System;
using Nebula.Net.Data;

namespace Nebula.Core.Online.Events
{
    public class NewUserMessageEventArgs : EventArgs
    {
        public NewUserMessageEventArgs(NetUserInfo userInfo, NetMessage message)
        {
            User = userInfo;
            Message = message;
        }

        public NetUserInfo User    { get; }
        public NetMessage  Message { get; }
    }
}