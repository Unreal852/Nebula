using Nebula.Core.Providers.Youtube;
using Nebula.Model;

namespace Nebula.ViewModel.Dialogs
{
    public class PlaylistImportDialogViewModel : BaseDialogViewModel
    {
        private string _playlistPath;
        private bool   _keepSync;

        public PlaylistImportDialogViewModel()
        {
            Title = NebulaClient.GetLang("Create Playlist");
        }

        public string PlaylistPath
        {
            get => _playlistPath;
            set
            {
                Set(ref _playlistPath, value);
                OnPropertyChanged(nameof(KeepSyncVisible));
            }
        }

        public bool KeepSync
        {
            get => _keepSync;
            set => Set(ref _keepSync, value);
        }

        public bool KeepSyncVisible => PlaylistPath?.StartsWith("http") ?? false;

        protected override async void OnConfirm()
        {
            if (string.IsNullOrWhiteSpace(PlaylistPath))
                return;
            Playlist playlist = await NebulaClient.Providers.FindProviderByType<YoutubeMediaProvider>().GetPlaylist(PlaylistPath);
            playlist.ValidateFields();
            NebulaClient.Playlists.AddPlaylist(playlist);
            Close();
        }
    }
}