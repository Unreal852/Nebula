using System;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteMVVM;
using LiteMVVM.Command;
using LiteMVVM.Navigation;
using Nebula.Model;
using Nebula.Utils.Collections.Paging;

namespace Nebula.ViewModel
{
    public class PlaylistViewModel : BaseViewModel, INavigable
    {
        private ObservableFilterPager<MediaInfo> _pager;

        public PlaylistViewModel()
        {
            PlayMediaCommand = new AsyncRelayCommand<MediaInfo>(PlayMedia);
            RemoveMediaCommand = new AsyncRelayCommand<MediaInfo>(RemoveMedia);
            SetIsActiveCommand = new AsyncRelayCommand<MediaInfo>(SetIsActive);
            FilterMediasCommand = new RelayCommand<string>(FilterMedias);
            Medias.PageChanged += (_, _) => OnPropertyChanged(nameof(CurrentPage));
            Medias.TotalPagesChanged += (_, _) => OnPropertyChanged(nameof(TotalPages));
        }

        public Playlist Playlist            { get; private set; }
        public ICommand PlayMediaCommand    { get; }
        public ICommand RemoveMediaCommand  { get; }
        public ICommand SetIsActiveCommand  { get; }
        public ICommand FilterMediasCommand { get; }

        public int TotalPages => Medias?.TotalPages ?? 0;

        public ObservableFilterPager<MediaInfo> Medias { get; } = new(null);

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

        public void OnNavigated(object param)
        {
            if (param is Playlist playlist)
            {
                Playlist = playlist;
                Medias.SetSource(playlist.Medias);
                OnPropertiesChanged(nameof(Name), nameof(Description), nameof(Author), nameof(Thumbnail), nameof(CurrentPage), nameof(TotalPages), nameof(Medias));
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