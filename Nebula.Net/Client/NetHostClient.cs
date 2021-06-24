using System.Net;
using System.Threading.Tasks;
using Nebula.Net.Helper;
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
        public IPAddress IpAddress { get; private set; }

        public void Connect(string ip, int port, string key = "")
        {
            Client.Connect(ip, port, key);
        }

        public async Task HostAndConnect(NetServerSettings settings)
        {
            if (!await Server.Start(settings))
                return;
            if (settings.UseUpnp)
                IpAddress = await Server.NatDevice.GetExternalIPAsync();
            else
                IpAddress = await NetHelper.GetExternalIpAddress();
            Client.Connect("127.0.0.1", Server.Settings.ServerPort, Server.Settings.Key);
        }

        public async Task StopAndDisconnect()
        {
            if (Server.IsRunning)
                await Server.Stop();
            Client.Disconnect();
        }
    }
}