using System.Threading.Tasks;
using Nebula.Model;

namespace Nebula.Core.Providers.Youtube
{
    public class YoutubeArtistPlaylist : Playlist
    {
        public override async Task Load()
        {
            if (IsLoaded)
                return;
            
        }
    }
}