using System.Threading.Tasks;
using HandyControl.Controls;

namespace Nebula.Core.Playlists
{
    public class DefaultPlaylistMediasLoader : IPlaylistMediasLoader
    {
        public PlaylistMediasLoaderType LoaderType => PlaylistMediasLoaderType.Default;

        public Task<bool> LoadPlaylist(Playlist playlist)
        {
            Growl.Error($"Failed to load playlist '{playlist.Info.Name}'. Loader '{playlist.Info.LoaderType}' not found");
            return new Task<bool>(() => false);
        }
    }
}