using System.Globalization;
using System.Threading;
using System.Windows;

namespace Nebula
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnAppStart(object sender, StartupEventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture;
        }

        private async void OnAppExit(object sender, ExitEventArgs e)
        {
            await NebulaClient.Database.Vacuum();
        }
    }
}