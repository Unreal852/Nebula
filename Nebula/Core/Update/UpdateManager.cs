using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using HandyControl.Controls;
using HandyControl.Data;
using Octokit;

namespace Nebula.Core.Update
{
    public class UpdateManager
    {
        private const           string RepositoryOwner = "Unreal852";
        private const           string RepositoryName  = "Nebula";
        private const           string SetupFileName   = "nebula_setup.exe";
        private static readonly string AssemblyVersion;

        static UpdateManager()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            AssemblyVersion = version == null ? "0.0.0" : $"{version.Major}.{version.Minor}.{version.Build}";
        }

        public UpdateManager()
        {
            Client = new RepositoriesClient(new ApiConnection(new Connection(new ProductHeaderValue("Nebula"), GitHubClient.GitHubApiUrl)));
            WebClient = new WebClient();
            WebClient.DownloadFileCompleted += OnDownloadFileCompleted;
            if (File.Exists(SetupFileName))
                File.Delete(SetupFileName);
        }

        private RepositoriesClient Client    { get; }
        private WebClient          WebClient { get; }

        public async Task<UpdateCheckResult> CheckForUpdate()
        {
            Release release = await Client.Release.GetLatest(RepositoryOwner, RepositoryName);
            string releaseVersion = release.TagName.Split('-')[0].Replace("v", "");
            if (releaseVersion != AssemblyVersion)
                return new UpdateCheckResult(true, AssemblyVersion, releaseVersion, release.TagName, release.Assets[0].BrowserDownloadUrl);
            return new UpdateCheckResult(false, AssemblyVersion);
        }

        public async void CheckForUpdateNotify()
        {
            UpdateCheckResult result = await CheckForUpdate();
            if (result.UpdateAvailable)
            {
                GrowlInfo growlInfo = new GrowlInfo
                {
                    Type = InfoType.Ask,
                    Message = NebulaClient.GetLang("update_available", result.NewVersionFull),
                    ConfirmStr = NebulaClient.GetLang("update_update_now"),
                    CancelStr = NebulaClient.GetLang("dialog_no"),
                    StaysOpen = true,
                    ActionBeforeClose = isConfirmed =>
                    {
                        if (isConfirmed)
                            WebClient.DownloadFileAsync(new Uri(result.PackageUrl), SetupFileName);
                        return true;
                    }
                };
                Growl.Ask(growlInfo);
            }
        }

        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
#if RELEASE
            Process.Start(new ProcessStartInfo(SetupFileName) {UseShellExecute = true});
            System.Windows.Application.Current.Dispatcher.Invoke(() => System.Windows.Application.Current.Shutdown());
#endif
        }
    }
}