using Nebula.Desktop.DataTemplates;
using Nebula.Desktop.Services.AudioPlayer;
using Nebula.Desktop.Services.AudioPlayer.Controllers;
using Nebula.Desktop.Services.Theme;
using Nebula.Desktop.ViewModel;
using Nebula.Net.Services.Client;
using Nebula.Net.Services.Server;
using Nebula.Services.Localization;
using Serilog;
using Jab;
using Nebula.Net.Services;
using Nebula.Desktop.Contracts;
using Nebula.Desktop.Services.Medias;
using Nebula.Desktop.Services.Audio;
using Nebula.Desktop.Services.Logging;

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
[Singleton<INatMapperService, NatMapperService>]
[Singleton<INetServerService, NebulaServerService>]
[Singleton<INetClientService, NetClientService>]
[Singleton<LocalAudioPlayerController>]
[Singleton<RemoteAudioPlayerController>]
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
[Singleton<ViewModelPageBase, SettingsPageViewModel>]
[Singleton<ViewModelPageBase, LibraryPageViewModel>]
[Singleton<ViewModelPageBase, PlaylistPageViewModel>]
[Singleton<ViewModelPageBase, SearchResultsPageViewModel>]
file interface IViewModelServices
{
}