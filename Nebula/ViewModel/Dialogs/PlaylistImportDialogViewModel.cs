using HandyControl.Controls;
using HandyControl.Data;
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
            Title = NebulaClient.GetLang("playlist_import_title");
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
            Close();
            Growl.Info(new GrowlInfo
            {
                Message = NebulaClient.GetLang("playlist_importing"),
                ShowDateTime = false
            });
            Playlist playlist = await NebulaClient.Providers.FindProviderByType<YoutubeMediaProvider>().GetPlaylist(PlaylistPath);
            playlist.ValidateFields();
            NebulaClient.Playlists.AddPlaylist(playlist);
            Growl.Success(new GrowlInfo
            {
                Message = NebulaClient.GetLang("playlist_imported", playlist.Name),
                ShowDateTime = false
            });
        }
    }
}