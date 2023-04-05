using Nebula.Desktop.DataTemplates;
using Nebula.Desktop.Services.AudioPlayer;
using Nebula.Desktop.Services.AudioPlayer.Controllers;
using Nebula.Desktop.Services.Theme;
using Nebula.Desktop.ViewModel;
using Nebula.Net.Services.Client;
using Nebula.Net.Services.Server;
using Nebula.Services;
using Nebula.Services.Contracts;
using Nebula.Services.Audio;
using Nebula.Services.Localization;
using Nebula.Services.Logging;
using Nebula.Services.Medias;
using Serilog;
using Jab;

namespace Nebula.Desktop.Services;

[ServiceProvider]
[Import<IViewModelServices>]
[Singleton<ILogger>(Instance = nameof(LoggerService))]
#if DEBUG
[Singleton<AvaloniaLoggerService>]
#endif
[Singleton<IAppService, AppService>]
[Singleton<IUpdateService, UpdateService>]
[Singleton<ILanguageService, LanguageService>]
[Singleton<ISettingsService, SettingsService>]
[Singleton<IDatabaseService, RealmDatabaseService>]
[Singleton<IThemeService, ThemeService>]
[Singleton<IMediasProviderService, AllMediasProviderService>]
[Singleton<IMediasProviderService, LocalMediasProviderService>]
[Singleton<IMediasProviderService, YoutubeMediasProviderService>]
[Singleton<IAudioService, CsCoreAudioService>]
[Singleton<IAudioPlayerService, AudioPlayerService>]
[Singleton<INetServerService, NebulaServerService>]
[Singleton<INetClientService, NetClientService>]
[Singleton<LocalAudioPlayerController>]
[Transient<RemoteAudioPlayerController>]
public partial class ServiceProvider
{
    public ServiceProvider()
    {
    }

    private static ILogger LoggerService => Log.Logger;
}

[ServiceProviderModule]
[Singleton<ViewLocator>]
[Singleton<MainViewModel>]
[Singleton<TitleBarViewModel>]
[Singleton<PartyFlyoutViewModel>]
[Singleton<AudioPlayerViewModel>]
#if DEBUG
[Singleton<ViewModelPageBase, DevPageViewModel>]
#endif
[Singleton<ViewModelPageBase, SharedSessionPageViewModel>]
[Singleton<ViewModelPageBase, SettingsPageViewModel>]
[Singleton<ViewModelPageBase, LibraryPageViewModel>]
[Singleton<ViewModelPageBase, PlaylistPageViewModel>]
[Singleton<ViewModelPageBase, SearchResultsPageViewModel>]
file interface IViewModelServices
{
}