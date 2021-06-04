using System.Collections.Generic;
using System.Threading.Tasks;
using Nebula.Model;

namespace Nebula.Core.Database
{
    public class DatabasePlaylistLoader : IPlaylistLoader
    {
        public async Task<bool> LoadPlaylist(Playlist playlist)
        {
            List<MediaInfo> medias = await NebulaClient.Database.GetPlaylistMedias(playlist);
            playlist.Medias.AddRange(medias);
            return true;
        }
    }
}