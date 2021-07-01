using System.Collections.Generic;
using System.Threading.Tasks;
using Nebula.Core.Playlists;
using Nebula.Model;

namespace Nebula.Core.Database
{
    public class DatabasePlaylistMediasLoader : IPlaylistMediasLoader
    {
        public PlaylistMediasLoaderType LoaderType => PlaylistMediasLoaderType.Database;

        public async Task<bool> LoadPlaylist(Playlist playlist)
        {
            List<MediaInfo> medias = await NebulaClient.Database.GetPlaylistMedias(playlist);
            playlist.Medias.AddRange(medias);
            return true;
        }
    }
}