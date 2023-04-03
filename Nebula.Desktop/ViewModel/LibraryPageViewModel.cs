using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FluentAvalonia.UI.Controls;
using Nebula.Common.Playlist;
using Nebula.Desktop.ViewModel.Messages;
using Nebula.Services.Contracts;

namespace Nebula.Desktop.ViewModel;

public partial class LibraryPageViewModel : ViewModelPageBase
{
    private readonly IDatabaseService    _databaseService;
    private readonly IAudioPlayerService _audioPlayerService;

    public LibraryPageViewModel(IDatabaseService databaseService, IAudioPlayerService audioPlayerService)
    {
        _databaseService = databaseService;
        _audioPlayerService = audioPlayerService;
        PageName = "Library";
        PageIcon = Symbol.Library;
        Playlists = _databaseService.All<Playlist>();
    }

    public IQueryable<Playlist> Playlists { get; }

    [RelayCommand]
    public Task PlayPlaylist(Playlist playlist)
    {
        return _audioPlayerService.OpenPlaylist(playlist);
    }

    [RelayCommand]
    public void OpenPlaylist(Playlist playlist)
    {
        StrongReferenceMessenger.Default.Send(new PlaylistChangeMessage(playlist));
    }
}