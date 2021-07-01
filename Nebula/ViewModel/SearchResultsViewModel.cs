using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using LiteMVVM;
using LiteMVVM.Command;
using Nebula.Core.Playlists;
using Nebula.Model;
using Nebula.View.Controls;
using Nebula.View.Views.Dialogs;
using Nebula.ViewModel.Dialogs;

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
            AddToCommand = new AsyncRelayCommand<MediaInfo>(AddTo);
            OpenMediaCommand = new AsyncRelayCommand<MediaInfo>(OpenMedia);
        }

        public ICommand ScrollToTopCommand                { get; }
        public ICommand OpenMediaCommand                  { get; }
        public ICommand AddMediaToQueueCommand            { get; }
        public ICommand AddMediaToPlaylistCommand         { get; }
        public ICommand AddToCommand                      { get; }
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

        private async Task OpenMedia(MediaInfo media)
        {
            if (media == null)
                return;
            await NebulaClient.MediaPlayer.OpenMedia(media);
        }

        private void ShowPlaylistCreationDialog()
        {
            Dialog.Show<PlaylistCreationDialogView>();
        }

        private async Task AddTo(MediaInfo mediaInfo)
        {
            Dialog dialog = NebulaDialog.ShowDialog<ElementSelectorDialogView, LocalPlaylistSelectorViewModel>();
            var result = await dialog.GetResultAsync<Playlist>();
            result?.AddMedia(mediaInfo);
        }
    }
}