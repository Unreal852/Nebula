using System;
using System.Collections.ObjectModel;
using HandyControl.Controls;
using Nebula.Model;
using Nebula.MVVM;

namespace Nebula.ViewModel
{
    public class PlaylistViewModel : BaseViewModel
    {
        public PlaylistViewModel()
        {
        }

        public Playlist Playlist { get; private set; }

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
                Growl.Info(Playlist.Medias.CurrentPage + " / " + Playlist.Medias.TotalPages);
                OnPropertiesChanged(nameof(CurrentPage), nameof(Medias));
            }
        }

        public int TotalPages => Playlist.Medias.TotalPages - 1;

        public ObservableCollection<MediaInfo> Medias => Playlist?.Medias.PageElements;

        public override void OnNavigating(object param)
        {
            if (param is Playlist playlist)
            {
                Playlist = playlist;
                OnPropertiesChanged(nameof(Name), nameof(Description), nameof(Author), nameof(Thumbnail), nameof(Medias), nameof(CurrentPage), nameof(TotalPages));
            }
        }
    }
}