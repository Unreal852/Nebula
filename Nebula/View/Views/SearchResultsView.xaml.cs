using System.Windows.Controls;
using Nebula.Core.Playlists;
using Nebula.ViewModel;

namespace Nebula.View.Views
{
    public partial class SearchResultsView : UserControl
    {
        public SearchResultsView()
        {
            InitializeComponent();
        }

        private void OnContextMenuOpening(object sender, ContextMenuEventArgs e) // Do this in viewmodel
        {
            AddToPlaylist.Items.Clear();
            SearchResultsViewModel viewModel = (SearchResultsViewModel) DataContext;
            foreach (Playlist playlist in NebulaClient.Playlists.Playlists)
            {
                MenuItem menuItem = new MenuItem {Header = playlist.Info.Name, Command = viewModel.AddMediaToPlaylistCommand, CommandParameter = playlist};
                AddToPlaylist.Items.Add(menuItem);
            }
        }
    }
}