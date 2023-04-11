using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using Nebula.Common.Medias;
using Nebula.Common.Playlist;
using Nebula.Desktop.Contracts;
using Nebula.Desktop.Properties;
using Nebula.Desktop.Services.Medias;

namespace Nebula.Desktop.ViewModel;

public sealed partial class DevPageViewModel : ViewModelPageBase
{
    private readonly IAudioPlayerService _audioPlayerService;
    private readonly IDatabaseService _databaseService;

    [ObservableProperty]
    private bool _isClientConnected;

    [ObservableProperty]
    private bool _isServerHost;

    [ObservableProperty]
    private string _playlistUrl = string.Empty;

    [ObservableProperty]
    private string _videoUrl = string.Empty;

    [ObservableProperty]
    private string _updateStatus = string.Empty;

    public DevPageViewModel(IAudioPlayerService audioPlayerService, IDatabaseService databaseService)
    {
        PageName = Resources.PageDev;
        PageIcon = Symbol.Code;
        PageIsDefault = true;
        _audioPlayerService = audioPlayerService;
        _databaseService = databaseService;
#if DEBUG
        VideoUrl = "https://www.youtube.com/watch?v=S2dRcipMCpw";
        PlaylistUrl = "https://www.youtube.com/playlist?list=PLJQAkXxjQzkNLdDzPuLYvJ-iRqV8S9hip";
#endif
    }

    [RelayCommand]
    private Task RequestVideoChange()
    {
        if (string.IsNullOrWhiteSpace(VideoUrl))
            return Task.CompletedTask;
        return _audioPlayerService.OpenMedia(MediaInfo.FromId(VideoUrl));
    }

    [RelayCommand]
    private async Task RequestPlaylistChange()
    {
        if (string.IsNullOrWhiteSpace(PlaylistUrl))
            return;
        var providers = Ioc.Default.GetService<IEnumerable<IMediasProviderService>>();
        var youtubeProvider = providers!.Single(p => p is YoutubeMediasProviderService);
        var playlist = await youtubeProvider.GetPlaylistAsync(PlaylistUrl);
        if (playlist == null)
            return;
        await _audioPlayerService.OpenPlaylist(playlist);
    }

    [RelayCommand]
    private async Task FillDatabase()
    {
        var providers = Ioc.Default.GetService<IEnumerable<IMediasProviderService>>();
        var youtubeProvider = providers!.Single(p => p is YoutubeMediasProviderService);

        var playlist = await youtubeProvider.GetPlaylistAsync(PlaylistUrl);
        if (playlist != null)
            await _databaseService.AddAsync(playlist);
    }

    [RelayCommand]
    private async Task LoadDatabase()
    {
        var userPlaylist = _databaseService.All<Playlist>().FirstOrDefault();
        if (userPlaylist == null)
            return;
        await Ioc.Default.GetRequiredService<IAudioPlayerService>().OpenMedia(userPlaylist.Medias[3]);
    }
}