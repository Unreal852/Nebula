using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using HandyControl.Controls;
using LiteMVVM;
using LiteMVVM.Command;
using Nebula.Model;
using Nebula.View;
using Nebula.View.Views;
using Nebula.View.Views.Dialogs;
using Nebula.ViewModel.Dialogs;
using RelayCommand = HandyControl.Tools.Command.RelayCommand;

namespace Nebula.ViewModel
{
    public class SideBarViewModel : BaseViewModel
    {
        public static SideBarViewModel Instance { get; private set; }

        private bool _showPlaylists = false;

        public SideBarViewModel()
        {
            Instance = this;
            NavigateCommand = new RelayCommand(MainWindowViewModel.Instance.Navigate);
            NavigateOnlineSessionCommand = new AsyncRelayCommand(NavigateOnlineSession);
            NavigateToPlaylistCommand = new RelayCommand<Playlist>(NavigateToPlaylist);
            CreatePlaylistCommand = new RelayCommand(CreatePlaylist);
            ImportPlaylistCommand = new RelayCommand<Playlist>(ImportPlaylist);
            PlayPlaylistCommand = new AsyncRelayCommand<Playlist>(PlayPlaylist);
            Playlists.CollectionChanged += (_, _) => ShowPlaylists = Playlists.Count > 0;
        }

        public ICommand                       NavigateCommand              { get; }
        public ICommand                       NavigateOnlineSessionCommand { get; }
        public ICommand                       NavigateToPlaylistCommand    { get; }
        public ICommand                       CreatePlaylistCommand        { get; }
        public ICommand                       ImportPlaylistCommand        { get; }
        public ICommand                       PlayPlaylistCommand          { get; }
        public ObservableCollection<Playlist> Playlists                    => NebulaClient.Playlists.Playlists;

        public bool ShowPlaylists
        {
            get => _showPlaylists;
            set => Set(ref _showPlaylists, value);
        }

        private void NavigateToPlaylist(Playlist playlist)
        {
            if (playlist == null)
                return;
            Messenger.Broadcast(this, NavigationInfo.Create(typeof(PlaylistView), playlist, true));
        }

        private void CreatePlaylist(object param)
        {
            Dialog.Show<PlaylistCreationDialogView>().DataContext = new PlaylistCreationDialogViewModel();
        }

        private void ImportPlaylist(object param)
        {
            NebulaClient.ShowDialog<PlaylistImportDialogView>();
        }

        private async Task PlayPlaylist(Playlist playlist)
        {
            await NebulaClient.MediaPlayer.OpenPlaylist(playlist);
        }

        private async Task NavigateOnlineSession()
        {
            NebulaClient.ShowDialog<OnlineSessionJoinCreateDialogView>();
        }
    }
}