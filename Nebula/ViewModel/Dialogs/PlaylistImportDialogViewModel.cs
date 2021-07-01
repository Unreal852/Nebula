using System.Threading.Tasks;
using System.Windows.Input;
using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools.Extension;
using LiteMVVM.Command;
using Nebula.Core.Playlists;
using Nebula.Core.Providers;
using Nebula.View.Views.Dialogs;

namespace Nebula.ViewModel.Dialogs
{
    public class PlaylistImportDialogViewModel : BaseDialogViewModel
    {
        private string _playlistPath;
        private bool   _keepSync;

        public PlaylistImportDialogViewModel()
        {
            Title = NebulaClient.GetLang("playlist_import_title");
            SearchPlaylistCommand = new AsyncRelayCommand(SearchPlaylist);
        }

        public ICommand SearchPlaylistCommand { get; }

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
            Playlist playlist = await NebulaClient.Providers.GetProvider(ProviderType.Youtube).GetPlaylist(PlaylistPath);
            if (!KeepSync)
                playlist.Info.LoaderType = PlaylistMediasLoaderType.Database;
            NebulaClient.Playlists.AddPlaylist(playlist);
            Growl.Success(new GrowlInfo
            {
                Message = NebulaClient.GetLang("playlist_imported", playlist.Info.Name),
                ShowDateTime = false
            });
        }

        private async Task SearchPlaylist()
        {
            Dialog dialog = NebulaDialog.ShowDialog<ElementSelectorDialogView, ImportPlaylistSearchSelectorViewModel>();
            var result = await dialog.GetResultAsync<Playlist>();
            if (result == null)
                return;
            PlaylistPath = NebulaClient.Providers.GetProvider(ProviderType.Youtube).GetPlaylistUrl(result);
        }
    }
}