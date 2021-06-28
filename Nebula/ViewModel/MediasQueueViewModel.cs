using System.Threading.Tasks;
using System.Windows.Input;
using HandyControl.Data;
using LiteMVVM;
using LiteMVVM.Command;
using Nebula.Model;
using Nebula.Utils.Collections.Paging;

namespace Nebula.ViewModel
{
    public class MediasQueueViewModel : BaseViewModel
    {
        public MediasQueueViewModel()
        {
            RemoveMediaFromQueueCommand = new RelayCommand<MediaInfo>(RemoveMediaFromQueue);
            PlayNowCommand = new AsyncRelayCommand<MediaInfo>(PlayNow);
            PageChangedCommand = new RelayCommand<FunctionEventArgs<int>>(PageChanged);
            Pager = new ObservableFilterPager<MediaInfo>(NebulaClient.MediaPlayer.MediaQueue.Queue, 1, 10);
            Pager.TotalPagesChanged += (_, _) => OnPropertyChanged(nameof(TotalPages));
            NebulaClient.MediaPlayer.MediaChanged += (_, _) => { OnPropertiesChanged(nameof(MediaTitle), nameof(MediaAuthor), nameof(MediaThumbnail)); };
        }

        public ICommand                         RemoveMediaFromQueueCommand { get; }
        public ICommand                         PlayNowCommand              { get; }
        public ICommand                         PageChangedCommand          { get; }
        public ObservableFilterPager<MediaInfo> Pager                       { get; }
        public MediaInfo                        CurrentMedia                => NebulaClient.MediaPlayer.CurrentMedia;
        public string                           MediaTitle                  => CurrentMedia?.Title ?? "";
        public string                           MediaAuthor                 => CurrentMedia?.Author ?? "";
        public string                           MediaThumbnail              => CurrentMedia?.AnyThumbnailFromHighest;
        public int                              TotalPages                  => Pager.TotalPages;


        private void PageChanged(FunctionEventArgs<int> e) => Pager.CurrentPage = e.Info;

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