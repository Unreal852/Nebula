using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Nebula.Model;
using Nebula.View.Helper;

namespace Nebula.ViewModel.Dialogs
{
    public class ImportPlaylistSearchSelectorViewModel : BaseElementSelectorDialogViewModel<Playlist>
    {
        public ImportPlaylistSearchSelectorViewModel()
        {
            Pager.SetSource(Playlists);
            Title = NebulaClient.GetLang("Select Playlist");
            ItemsTemplate = ResourcesHelper.GetResource<DataTemplate>("PlaylistItemTemplate");
            SearchBarVisible = true;
        }

        private ObservableCollection<Playlist> Playlists { get; } = new();

        protected override void OnConfirm()
        {
            if (SelectedElement is not Playlist playlist)
                return;
            Result = playlist;
            Close();
        }

        protected override async Task Search(string value)
        {
            if (!value.Contains("playlist"))
                value += " playlist";
            Playlists.Clear();
            await foreach (Playlist playlist in NebulaClient.Providers.SearchPlaylists(value))
                Playlists.Add(playlist);
        }
    }
}