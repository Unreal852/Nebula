using System.Threading;
using System.Windows;
using Nebula.Core.Extensions;
using Nebula.View;

namespace Nebula
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnAppStart(object sender, StartupEventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = NebulaClient.Settings.General.Language.GetCulture();
        }

        private void OnAppExit(object sender, ExitEventArgs e)
        {
            ViewModelsLocator.Instance.MainWindowViewModel.Navigate("exit");
            NebulaClient.Discord.ClearActivity();
            NebulaClient.OnlineSession.HostClient.StopAndDisconnect();
        }
    }
}