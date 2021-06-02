using LiteMVVM;
using Nebula.Media;
using Nebula.Model;

namespace Nebula.ViewModel
{
    public class MediasQueueViewModel : BaseViewModel
    {
        public MediasQueueViewModel()
        {
            NebulaClient.MediaPlayer.MediaChanged += (_, _) => { OnPropertiesChanged(nameof(MediaTitle), nameof(MediaAuthor)); };
        }

        public IMediaInfo CurrentMedia => NebulaClient.MediaPlayer.CurrentMedia;

        public string MediaTitle  => CurrentMedia?.Title ?? "Unknown";
        public string MediaAuthor => CurrentMedia?.Author ?? "Unknown";
    }
}