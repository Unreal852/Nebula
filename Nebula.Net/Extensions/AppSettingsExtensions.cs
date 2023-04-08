using Nebula.Common.Settings;

namespace Nebula.Net.Extensions;

public static class AppSettingsExtensions
{
    public static NetOptions GetNetOptions(this AppSettings appSettings)
    {
        return new NetOptions(appSettings.PartyServerIp, appSettings.PartyServerPort, 100, true);
    }
}
