using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using LiteMVVM;
using LiteMVVM.Command;
using LiteMVVM.Navigation;
using Nebula.Core.Playlists;
using Nebula.Model;
using Nebula.Utils.Collections.Paging;
using Nebula.View;
using Nebula.View.Views;

namespace Nebula.ViewModel
{
    public class PlaylistViewModel : BaseViewModel, INavigable
    {
        private Playlist _playlist;

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
                {
                    Medias.SetSource(Playlist.Medias);
                    Medias.PageSize = NebulaClient.Settings.General.PlaylistPageSize;
                }
                OnPropertyChanged();
                OnPropertiesChanged(nameof(Name), nameof(Description), nameof(Author), nameof(Thumbnail), nameof(Medias));
            }
        }

        public string Name
        {
            get => Playlist?.Info.Name;
            set
            {
                if (Playlist == null || Playlist.Info.Name == value)
                    return;
                Playlist.Info.Name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => Playlist?.Info.Description;
            set
            {
                if (Playlist == null || Playlist.Info.Name == value)
                    return;
                Playlist.Info.Description = value;
                OnPropertyChanged();
            }
        }

        public string Author
        {
            get => Playlist?.Info.AuthorName;
            set
            {
                if (Playlist == null || Playlist.Info.AuthorName == value)
                    return;
                Playlist.Info.AuthorName = value;
                OnPropertyChanged();
            }
        }

        public string Thumbnail
        {
            get => Playlist?.Info.AnyThumbnailFromHighest;
            set
            {
                if (Playlist == null)
                    return;
                Playlist.Info.CustomThumbnail = value;
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
                    await playlist.Load();
                OnPropertiesChanged(nameof(Duration), nameof(CurrentPage), nameof(TotalPages));
            }
        }

        public void OnLeft()
        {
            
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
            Dialog dialog = NebulaDialog.ShowWarningNoYes("dialog_delete_playlist_media");
            var result = await dialog.GetResultAsync<bool>();
            if (!result)
                return;
            Playlist.RemoveMedia(mediaInfo);
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
            Dialog dialog = NebulaDialog.ShowWarningNoYes("dialog_delete_playlist");
            var result = await dialog.GetResultAsync<bool>();
            if (!result)
                return;
            await NebulaClient.Playlists.DeletePlaylist(Playlist);
            Messenger.Broadcast(this, NavigationInfo.Create(typeof(HomeView), null, false));
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
            {
                Medias.ResetFilter();
            }
            else
            {
                string loweredFilter = filter.ToLower();
                string[] split = loweredFilter.Split(':');
                Predicate<MediaInfo> predicate = split[0] switch
                {
                    "by"      => media => media.AuthorName.ToLower().Contains(split[1]),
                    "active"  => media => media.IsActive,
                    "nactive" => media => !media.IsActive,
                    _         => media => media.Title.ToLower().Contains(loweredFilter)
                };

                Medias.ApplyFilter(predicate);
            }
        }
    }
}