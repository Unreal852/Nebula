using Nebula.Core.Playlists;
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
            Title = NebulaClient.GetLang("playlist_create_title");
        }

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

        protected override void OnConfirm()
        {
            var playlistInfo = new PlaylistInfo(PlaylistMediasLoaderType.Database, "", "user", PlaylistName, PlaylistAuthor, PlaylistDescription);
            NebulaClient.Playlists.AddPlaylist(new Playlist(playlistInfo));
            Close();
        }
    }
}