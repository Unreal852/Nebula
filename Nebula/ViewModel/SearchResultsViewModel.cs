using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using Nebula.Media;
using Nebula.Media.Player;
using Nebula.Model;
using Nebula.MVVM;
using Nebula.MVVM.Commands;
using Nebula.View.Controls;
using Nebula.View.Views.Dialogs;

namespace Nebula.ViewModel
{
    public class SearchResultsViewModel : BaseViewModel
    {
        public SearchResultsViewModel()
        {
            ScrollToTopCommand = new RelayCommand(ScrollToTop);
            AddMediaToQueueCommand = new RelayCommand<MediaInfo>(AddMediaToQueue);
            AddMediaToPlaylistCommand = new AsyncRelayCommand<Playlist>(AddMediaToPlaylist);
            ShowPlaylistCreationDialogCommand = new AsyncRelayCommand(ShowPlaylistCreationDialog);
            OpenMediaCommand = new AsyncRelayCommand<IMediaInfo>(OpenMedia);
        }

        public ICommand ScrollToTopCommand                { get; }
        public ICommand OpenMediaCommand                  { get; }
        public ICommand AddMediaToQueueCommand            { get; }
        public ICommand AddMediaToPlaylistCommand         { get; }
        public ICommand ShowPlaylistCreationDialogCommand { get; }

        public MediaInfo                       CurrentMedia { get; set; }
        public ObservableCollection<MediaInfo> Medias       { get; } = new();

        private void ScrollToTop(object obj)
        {
            if (obj is ListBoxEx listBoxEx && listBoxEx.Items.Count > 0)
                listBoxEx.ScrollIntoView(listBoxEx.Items[0]);
        }

        private void AddMediaToQueue(MediaInfo media)
        {
            if (media == null)
                return;
            NebulaClient.MediaPlayer.MediaQueue.Enqueue(media);
        }

        private async Task AddMediaToPlaylist(Playlist playlist)
        {
            if (playlist == null || CurrentMedia == null)
                return;
            playlist.AddMedia(CurrentMedia);
        }

        private async Task OpenMedia(IMediaInfo media)
        {
            if (media == null)
                return;
            Growl.Info(CurrentMedia?.Title);
            await NebulaClient.MediaPlayer.OpenMedia(media);
        }

        private async Task ShowPlaylistCreationDialog(object param)
        {
            Dialog.Show<PlaylistCreationDialogView>();
        }
    }
}