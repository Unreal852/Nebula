using Nebula.Common.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Net.Extensions;

public static class AppSettingsExtensions
{
    public static NetOptions GetNetOptions(this AppSettings appSettings)
    {
        return new NetOptions(appSettings.PartyServerIp, appSettings.PartyServerPort, 100, true);
    }
}
