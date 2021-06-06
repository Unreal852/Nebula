using System.Collections.Generic;
using System.Windows.Input;
using HandyControl.Controls;
using LiteMVVM.Command;
using Nebula.Model;

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

        private void CreatePlaylist()
        {
            NebulaClient.Playlists.AddPlaylist(new Playlist(PlaylistName, PlaylistDescription, PlaylistAuthor,
                "", "", "",
                new List<MediaInfo>()));
            Close();
        }

        protected override void OnConfirm()
        {
            Growl.Info("TEST");
        }
    }
}