using Nebula.Net.Data;

namespace Nebula.Model
{
    public class UserMessage
    {
        public UserMessage(NetUserInfo sender, NetMessage message)
        {
            Sender = sender;
            Message = message;
        }

        public NetUserInfo Sender  { get; }
        public NetMessage  Message { get; }
    }
}