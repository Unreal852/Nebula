using System.Threading.Tasks;
using Nebula.Net.Server;

namespace Nebula.Net.Client
{
    public class NetHostClient
    {
        public NetHostClient()
        {
            Server = new NetServer();
            Client = new NetClient();
        }

        public NetServer Server    { get; }
        public NetClient Client    { get; }
        public string    IpAddress { get; private set; }

        public void Connect(string ip, int port, string key = "")
        {
            Client.Connect(ip, port, key);
        }

        public async Task HostAndConnect(NetServerSettings settings)
        {
            if (!await Server.Start(settings))
                return;
            IpAddress = (await Server.NatDevice.GetExternalIPAsync()).ToString();
            Client.Connect("127.0.0.1", Server.Settings.ServerPort, Server.Settings.Key);
        }

        public async Task DisconnectAndStop()
        {
            Client.Disconnect();
            await Server.Stop();
        }
    }
}