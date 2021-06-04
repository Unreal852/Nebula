using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using LiteMVVM;
using LiteMVVM.Command;
using LiteMVVM.Navigation;
using Nebula.Model;
using Nebula.Utils.Collections.Paging;
using Nebula.View;
using Nebula.View.Views;

namespace Nebula.ViewModel
{
    public class PlaylistViewModel : BaseViewModel, INavigable
    {
        private ObservableFilterPager<MediaInfo> _pager;
        private Playlist                         _playlist;

        public PlaylistViewModel()
        {
            PlayMediaCommand = new AsyncRelayCommand<MediaInfo>(PlayMedia);
            RemoveMediaCommand = new AsyncRelayCommand<MediaInfo>(RemoveMedia);
            SetIsActiveCommand = new AsyncRelayCommand<MediaInfo>(SetIsActive);
            DeletePlaylistCommand = new AsyncRelayCommand(DeletePlaylist);
            FilterMediasCommand = new RelayCommand<string>(FilterMedias);
            TextChangedCommand = new RelayCommand<string>(OnTextChanged);
            Medias.PageChanged += (_, _) => OnPropertyChanged(nameof(CurrentPage));
            Medias.TotalPagesChanged += (_, _) => OnPropertyChanged(nameof(TotalPages));
        }

        public ICommand PlayMediaCommand      { get; }
        public ICommand RemoveMediaCommand    { get; }
        public ICommand SetIsActiveCommand    { get; }
        public ICommand FilterMediasCommand   { get; }
        public ICommand DeletePlaylistCommand { get; }
        public ICommand TextChangedCommand    { get; }

        public int      TotalPages => Medias?.TotalPages ?? 0;
        public TimeSpan Duration   => Playlist?.TotalDuration ?? TimeSpan.Zero;

        public ObservableFilterPager<MediaInfo> Medias { get; } = new(null);

        public Playlist Playlist
        {
            get => _playlist;
            set
            {
                _playlist = value;
                if (Playlist != null)
                    Medias.SetSource(Playlist.Medias);
                OnPropertyChanged();
                OnPropertiesChanged(nameof(Name), nameof(Description), nameof(Author), nameof(Thumbnail), nameof(Medias));
            }
        }

        public string Name
        {
            get => Playlist?.Name;
            set
            {
                if (Playlist == null || Playlist.Name == value)
                    return;
                Playlist.Name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => Playlist?.Description;
            set
            {
                if (Playlist == null || Playlist.Name == value)
                    return;
                Playlist.Description = value;
                OnPropertyChanged();
            }
        }

        public string Author
        {
            get => Playlist?.Author;
            set
            {
                if (Playlist == null || Playlist.Name == value)
                    return;
                Playlist.Author = value;
                OnPropertyChanged();
            }
        }

        public Uri Thumbnail
        {
            get => Playlist?.Thumbnail;
            set
            {
                if (Playlist == null || Playlist.Thumbnail == value)
                    return;
                Playlist.Thumbnail = value;
                OnPropertyChanged();
            }
        }

        public int CurrentPage
        {
            get => Medias.CurrentPage;
            set
            {
                Medias.CurrentPage = value;
                OnPropertyChanged();
            }
        }

        public async void OnNavigated(object param)
        {
            if (param is Playlist playlist)
            {
                Playlist = playlist;
                if (!Playlist.IsLoaded)
                {
                    await playlist.Load();
                }

                OnPropertiesChanged(nameof(Duration), nameof(CurrentPage), nameof(TotalPages));
            }
        }

        private async Task PlayMedia(MediaInfo mediaInfo)
        {
            if (mediaInfo == null)
                return;
            await NebulaClient.MediaPlayer.OpenMedia(mediaInfo);
        }

        private async Task RemoveMedia(MediaInfo mediaInfo)
        {
            if (Playlist == null || mediaInfo == null)
                return;
            Playlist.Medias.Remove(mediaInfo);
            await NebulaClient.Database.RemovePlaylistMedia(Playlist, mediaInfo);
        }

        private async Task SetIsActive(MediaInfo mediaInfo)
        {
            if (Playlist == null || mediaInfo == null)
                return;
            await NebulaClient.Database.UpdatePlaylistMedia(Playlist, mediaInfo);
        }

        private async Task DeletePlaylist()
        {
            if (Playlist == null)
                return;
            await NebulaClient.Playlists.DeletePlaylist(Playlist);
            Messenger.Broadcast(this, NavigationInfo.Create(typeof(TestControl1), null, false));
        }

        private void OnTextChanged(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                FilterMedias(null);
        }

        private void FilterMedias(string filter)
        {
            if (Playlist == null)
                return;
            if (string.IsNullOrWhiteSpace(filter))
                Medias.ResetFilter();
            else
            {
                string loweredFilter = filter.ToLower();
                string[] split = loweredFilter.Split(':');
                Predicate<MediaInfo> predicate = split[0] switch
                {
                    "by"      => media => media.Author.ToLower().Contains(split[1]),
                    "active"  => media => media.IsActive,
                    "nactive" => media => !media.IsActive,
                    _         => media => media.Title.ToLower().Contains(loweredFilter)
                };

                Medias.ApplyFilter(predicate);
            }
        }
    }
}