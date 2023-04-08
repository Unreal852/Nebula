using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FluentAvalonia.UI.Controls;
using Nebula.Common.Medias;
using Nebula.Common.Playlist;
using Nebula.Common.Utils;
using Nebula.Desktop.ViewModel.Messages;
using Nebula.Services.Contracts;
using Serilog;

namespace Nebula.Desktop.ViewModel;

public sealed partial class PlaylistPageViewModel : ViewModelPageBase, IRecipient<PlaylistChangeMessage>
{
    private readonly IDatabaseService _databaseService;
    private readonly IAudioPlayerService _audioPlayerService;

    private Pager<MediaInfo> _pager = default!;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name), nameof(Description), nameof(Author), nameof(Medias))]
    private Playlist _playlist = default!;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Medias))]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _maxPage = 1;

    [ObservableProperty]
    private string _searchFilter = string.Empty;

    public PlaylistPageViewModel(IDatabaseService databaseService, IAudioPlayerService audioPlayerService)
    {
        _databaseService = databaseService;
        _audioPlayerService = audioPlayerService;
        PageIcon = Symbol.ListFilled;
        PageIsAlwaysVisible = false;

        StrongReferenceMessenger.Default.RegisterAll(this);
    }

    public string Name => Playlist.Name;
    public string? Description => Playlist.Description;
    public string? Author => Playlist.Author;

    public IEnumerable<MediaInfo> Medias => string.IsNullOrWhiteSpace(SearchFilter)
            ? _pager.PageElements
            : _pager.ApplyPaging(_pager.Source.Where(m =>
                    m.Title != null && m.Title.Contains(SearchFilter, StringComparison.InvariantCultureIgnoreCase)));

    partial void OnPlaylistChanged(Playlist value)
    {
        _pager = new Pager<MediaInfo>(Playlist.Medias);
        CurrentPage = _pager.CurrentPage;
        MaxPage = _pager.MaxPage;
        PageName = value.Name;
    }

    [RelayCommand]
    private void PreviousPage()
    {
        var newPage = _pager.PreviousPage();
        if (newPage != CurrentPage)
            CurrentPage = newPage;
    }

    [RelayCommand]
    private void NextPage()
    {
        var newPage = _pager.NextPage();
        if (newPage != CurrentPage)
            CurrentPage = newPage;
    }

    [RelayCommand]
    private Task PlayMedia(IMediaInfo mediaInfo)
    {
        return _audioPlayerService.OpenMedia(mediaInfo);
    }

    [RelayCommand]
    private Task DeleteMedia(IMediaInfo mediaInfo)
    {
        if (mediaInfo is MediaInfo media)
        {
            return _databaseService.EnterWriteTransaction(() => { Playlist.Medias.Remove(media); });
            //  Refresh();
        }

        return Task.CompletedTask;
    }

    [RelayCommand]
    private void SearchMedias()
    {
        OnPropertyChanged(nameof(Medias));
    }

    private void Refresh()
    {
        OnPropertyChanged(nameof(Medias));
        _pager.Refresh();
        MaxPage = _pager.MaxPage;
    }

    public void Receive(PlaylistChangeMessage message)
    {
        StrongReferenceMessenger.Default.Send(new PageChangeMessage(this));
        Playlist = message.Value;
    }
}