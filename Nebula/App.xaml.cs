using System.Globalization;
using System.Threading;
using System.Windows;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Nebula
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnAppStart(object sender, StartupEventArgs e)
        {
            AppCenter.Start("df3a859e-110a-43b2-892d-71f4650c9c70", typeof(Analytics), typeof(Crashes));
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture;
        }

        private void OnAppExit(object sender, ExitEventArgs e)
        {
            NebulaClient.Discord.ClearActivity();
            NebulaClient.OnlineSession.HostClient.DisconnectAndStop();
        }
    }
}