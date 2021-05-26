using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using HandyControl.Controls;
using Nebula.Core;
using Nebula.Media;
using Nebula.Model;
using Nebula.MVVM;
using Nebula.MVVM.Commands;

namespace Nebula.ViewModel
{
    public class PlaylistViewModel : BaseViewModel
    {
        public PlaylistViewModel()
        {
            ChangePageCommand = new RelayCommand<int>(ChangePage);
        }

        public Playlist Playlist          { get; private set; }
        public ICommand ChangePageCommand { get; }

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
                Growl.Info(Playlist.Medias.CurrentPage + "");
                OnPropertiesChanged(nameof(CurrentPage), nameof(Medias));
            }
        }

        public int TotalPages => Playlist.Medias.TotalPages;

        public ObservableCollection<MediaInfo> Medias { get; private set; }

        public override void OnNavigating(object param)
        {
            if (param is Playlist playlist)
            {
                Playlist = playlist;
                OnPropertiesChanged(nameof(Name), nameof(Description), nameof(Author), nameof(Thumbnail), nameof(Medias), nameof(CurrentPage), nameof(TotalPages));
            }
        }

        private void ChangePage(int page)
        {
            CurrentPage = page;
        }
    }
}