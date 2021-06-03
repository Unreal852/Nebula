using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LiteMVVM;
using LiteMVVM.Command;
using Nebula.Media;
using Nebula.Model;

namespace Nebula.ViewModel
{
    public class MediasQueueViewModel : BaseViewModel
    {
        public MediasQueueViewModel()
        {
            RemoveMediaFromQueueCommand = new RelayCommand<MediaInfo>(RemoveMediaFromQueue);
            PlayNowCommand = new AsyncRelayCommand<MediaInfo>(PlayNow);
            PageChangedCommand = new RelayCommand<FunctionEventArgs<int>>(PageChanged);
            NebulaClient.MediaPlayer.MediaChanged += (_, _) => { OnPropertiesChanged(nameof(MediaTitle), nameof(MediaAuthor), nameof(MediaThumbnail)); };
            NebulaClient.MediaPlayer.MediaQueue.Queue.TotalPagesChanged += (_, e) => OnPropertyChanged(nameof(TotalPages));
        }

        public IMediaInfo CurrentMedia => NebulaClient.MediaPlayer.CurrentMedia;

        public ICommand                        RemoveMediaFromQueueCommand { get; }
        public ICommand                        PlayNowCommand              { get; }
        public ICommand                        PageChangedCommand          { get; }
        public string                          MediaTitle                  => CurrentMedia?.Title ?? "";
        public string                          MediaAuthor                 => CurrentMedia?.Author ?? "";
        public string                          MediaThumbnail              => CurrentMedia?.MediumResThumbnailUrl;
        public int                             TotalPages                  => NebulaClient.MediaPlayer.MediaQueue.Queue.TotalPages - 1;
        public ObservableCollection<MediaInfo> MediasQueue                 => NebulaClient.MediaPlayer.MediaQueue.Queue.PageElements;

        private void PageChanged(FunctionEventArgs<int> e)
        {
            NebulaClient.MediaPlayer.MediaQueue.Queue.CurrentPage = e.Info;
            Growl.Info(NebulaClient.MediaPlayer.MediaQueue.Queue.CurrentPage + "");
        }

        private void RemoveMediaFromQueue(MediaInfo mediaInfo)
        {
            if (mediaInfo == null)
                return;
            NebulaClient.MediaPlayer.MediaQueue.Remove(mediaInfo);
        }

        private async Task PlayNow(MediaInfo mediaInfo)
        {
            if (mediaInfo == null || !NebulaClient.MediaPlayer.MediaQueue.IsQueued(mediaInfo))
                return;
            RemoveMediaFromQueue(mediaInfo);
            await NebulaClient.MediaPlayer.OpenMedia(mediaInfo);
        }
    }
}