using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteMVVM;
using LiteMVVM.Command;
using Nebula.Core.Playlists;
using Nebula.Core.Settings;
using Nebula.View;
using Nebula.View.Views;
using Nebula.View.Views.Dialogs;
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
            Messenger.Subscribe<UserProfileSettings>((_, _) => OnPropertiesChanged(nameof(ProfileUsername), nameof(ProfileAvatar)));
            Playlists.CollectionChanged += (_, _) => ShowPlaylists = Playlists.Count > 0;
        }

        public ICommand                       NavigateCommand              { get; }
        public ICommand                       NavigateOnlineSessionCommand { get; }
        public ICommand                       NavigateToPlaylistCommand    { get; }
        public ICommand                       CreatePlaylistCommand        { get; }
        public ICommand                       ImportPlaylistCommand        { get; }
        public ICommand                       PlayPlaylistCommand          { get; }
        public ObservableCollection<Playlist> Playlists                    => NebulaClient.Playlists.Playlists;
        public string                         ProfileUsername              => NebulaClient.Settings.UserProfile.UserName;
        public string                         ProfileAvatar                => NebulaClient.Settings.UserProfile.AvatarUrl;

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
            NebulaDialog.ShowDialog<PlaylistCreationDialogView>();
        }

        private void ImportPlaylist(object param)
        {
            NebulaDialog.ShowDialog<PlaylistImportDialogView>();
        }

        private async Task PlayPlaylist(Playlist playlist)
        {
            await NebulaClient.MediaPlayer.OpenPlaylist(playlist);
        }

        private async Task NavigateOnlineSession()
        {
            if (NebulaClient.OnlineSession.IsClientConnected)
                Messenger.Broadcast(this, NavigationInfo.Create(typeof(OnlineSessionView), null, false));
            else
                NebulaDialog.ShowDialog<OnlineSessionJoinCreateDialogView>();
        }
    }
}