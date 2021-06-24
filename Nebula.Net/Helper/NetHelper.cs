using System;
using System.Net;
using System.Threading.Tasks;

namespace Nebula.Net.Helper
{
    public static class NetHelper
    {
        private const string IpInfoUrl = "https://ipinfo.io/ip";

        public static async Task<IPAddress> GetExternalIpAddress()
        {
            WebClient client = new WebClient();
            string externalIp = await client.DownloadStringTaskAsync(new Uri(IpInfoUrl));
            return IPAddress.Parse(externalIp);
        }
    }
}