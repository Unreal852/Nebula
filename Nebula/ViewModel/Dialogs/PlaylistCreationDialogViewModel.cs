using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HandyControl.Controls;
using HandyControl.Interactivity;
using Nebula.Core;
using Nebula.Media;
using Nebula.Model;
using Nebula.MVVM;
using Nebula.MVVM.Commands;

namespace Nebula.ViewModel.Dialogs
{
    public class PlaylistCreationDialogViewModel : BaseDialogViewModel
    {
        private string _playlistName;
        private string _playlistDescription;
        private string _playlistAuthor;

        public PlaylistCreationDialogViewModel()
        {
            CreatePlaylistCommand = new RelayCommand(CreatePlaylist);
        }

        public ICommand CreatePlaylistCommand { get; }

        public string PlaylistName
        {
            get => _playlistName;
            set => Set(ref _playlistName, value);
        }

        public string PlaylistDescription
        {
            get => _playlistDescription;
            set => Set(ref _playlistDescription, value);
        }

        public string PlaylistAuthor
        {
            get => _playlistAuthor;
            set => Set(ref _playlistAuthor, value);
        }

        private void CreatePlaylist(object obj)
        {
            NebulaClient.Playlists.AddPlaylist(new Playlist(PlaylistName, PlaylistDescription, PlaylistAuthor,
                new Uri("https://upload.wikimedia.org/wikipedia/commons/f/fa/Billie_Eilish_2019_by_Glenn_Francis_%28cropped%29_2.jpg"),
                new List<MediaInfo>()));
            Close();
        }
        
        protected override void OnConfirm(object param)
        {
            Growl.Info("TEST");
        }
    }
}