using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;
using Nebula.Media;
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
            foreach (IPlaylist playlist in NebulaClient.Playlists.Playlists)
            {
                MenuItem menuItem = new MenuItem {Header = playlist.Name, Command = viewModel.AddMediaToPlaylistCommand, CommandParameter = playlist};
                AddToPlaylist.Items.Add(menuItem);
            }
        }
    }
}