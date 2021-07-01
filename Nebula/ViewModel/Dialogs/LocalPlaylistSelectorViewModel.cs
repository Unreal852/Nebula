using System.Threading.Tasks;
using System.Windows;
using Nebula.Core.Playlists;
using Nebula.View.Helper;

namespace Nebula.ViewModel.Dialogs
{
    public class LocalPlaylistSelectorViewModel : BaseElementSelectorDialogViewModel<Playlist>
    {
        public LocalPlaylistSelectorViewModel()
        {
            Pager.SetSource(NebulaClient.Playlists.Playlists);
            Title = NebulaClient.GetLang("Select Playlist");
            ItemsTemplate = ResourcesHelper.GetResource<DataTemplate>("PlaylistItemTemplate");
            SearchBarVisible = true;
        }

        protected override void OnConfirm()
        {
            if (SelectedElement is not Playlist playlist)
                return;
            Result = playlist;
            Close();
        }

        protected override Task Search(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length == 0)
            {
                Pager.ResetFilter();
            }
            else
            {
                value = value.ToLower();
                Pager.ApplyFilter(playlist => playlist.Info.Name.ToLower().Contains(value));
            }

            return null;
        }
    }
}