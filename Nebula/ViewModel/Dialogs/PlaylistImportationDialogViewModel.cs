using HandyControl.Controls;
using Nebula.Core.Providers.Youtube;
using Nebula.Model;
using Nebula.View.Builder;
using Nebula.View.Builder.Attributes;

namespace Nebula.ViewModel.Dialogs
{
    public class PlaylistImportationDialogViewModel : BaseDialogViewModel
    {
        public static ViewCache Cache = ViewBuilder.BuildDialogViewCacheFromViewModel<PlaylistImportationDialogViewModel>();

        public PlaylistImportationDialogViewModel()
        {
            Cache.PrepareFor(this);
        }

        [Element(typeof(TextBox), "TextProperty"), Title("Playlist Url"), Size(250)]
        public string PlaylistPath { get; set; }

        protected override async void OnConfirm(object param)
        {
            Playlist playlist = await NebulaClient.Providers.FindProviderByType<YoutubeMediaProvider>().GetPlaylistt(PlaylistPath);
            playlist.ValidateFields();
            NebulaClient.Playlists.AddPlaylist(playlist);
            Close();
        }
    }
}