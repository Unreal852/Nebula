using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using HandyControl.Controls;
using Nebula.Model;
using Nebula.MVVM;
using Nebula.MVVM.Commands;

namespace Nebula.ViewModel
{
    public class PlaylistViewModel : BaseViewModel
    {
        public PlaylistViewModel()
        {
            PlayMediaCommand = new AsyncRelayCommand<MediaInfo>(PlayMedia);
            RemoveMediaCommand = new AsyncRelayCommand<MediaInfo>(RemoveMedia);
            SetIsActiveCommand = new AsyncRelayCommand<MediaInfo>(SetIsActive);
        }

        public Playlist Playlist           { get; private set; }
        public ICommand PlayMediaCommand   { get; }
        public ICommand RemoveMediaCommand { get; }
        public ICommand SetIsActiveCommand { get; }

        public int                             TotalPages => Playlist.Medias.TotalPages - 1;
        public ObservableCollection<MediaInfo> Medias     => Playlist?.Medias.PageElements;

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
            get => Playlist.Medias.CurrentPage;
            set
            {
                Playlist.Medias.CurrentPage = value;
                OnPropertiesChanged(nameof(CurrentPage), nameof(Medias));
            }
        }

        public override void OnNavigating(object param)
        {
            if (param is Playlist playlist)
            {
                Playlist = playlist;
                OnPropertiesChanged(nameof(Name), nameof(Description), nameof(Author), nameof(Thumbnail), nameof(Medias), nameof(CurrentPage), nameof(TotalPages));
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
    }
}