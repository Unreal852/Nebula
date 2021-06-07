using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using HandyControl.Controls;
using LiteMVVM;
using LiteMVVM.Command;
using Nebula.Media;
using Nebula.Model;
using Nebula.View.Controls;
using Nebula.View.Views.Dialogs;

namespace Nebula.ViewModel
{
    public class SearchResultsViewModel : BaseViewModel
    {
        public SearchResultsViewModel()
        {
            ScrollToTopCommand = new RelayCommand<ListBoxEx>(ScrollToTop);
            AddMediaToQueueCommand = new RelayCommand<MediaInfo>(AddMediaToQueue);
            AddMediaToPlaylistCommand = new RelayCommand<Playlist>(AddMediaToPlaylist);
            ShowPlaylistCreationDialogCommand = new RelayCommand(ShowPlaylistCreationDialog);
            OpenMediaCommand = new AsyncRelayCommand<IMediaInfo>(OpenMedia);
        }

        public ICommand ScrollToTopCommand                { get; }
        public ICommand OpenMediaCommand                  { get; }
        public ICommand AddMediaToQueueCommand            { get; }
        public ICommand AddMediaToPlaylistCommand         { get; }
        public ICommand ShowPlaylistCreationDialogCommand { get; }

        public MediaInfo                       CurrentMedia { get; set; }
        public ObservableCollection<MediaInfo> Medias       { get; } = new();

        private void ScrollToTop(ListBoxEx listBoxEx)
        {
            if (listBoxEx != null && listBoxEx.Items.Count > 0)
                listBoxEx.ScrollIntoView(listBoxEx.Items[0]);
        }

        private void AddMediaToQueue(MediaInfo media)
        {
            if (media == null)
                return;
            NebulaClient.MediaPlayer.MediaQueue.Enqueue(media);
        }

        private void AddMediaToPlaylist(Playlist playlist)
        {
            if (playlist == null || CurrentMedia == null)
                return;
            playlist.AddMedia(CurrentMedia);
        }

        private async Task OpenMedia(IMediaInfo media)
        {
            if (media == null)
                return;
            await NebulaClient.MediaPlayer.OpenMedia(media);
        }

        private void ShowPlaylistCreationDialog()
        {
            Dialog.Show<PlaylistCreationDialogView>();
        }
    }
}