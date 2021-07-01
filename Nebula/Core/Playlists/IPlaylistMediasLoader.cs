using System.Threading.Tasks;

namespace Nebula.Core.Playlists
{
    public interface IPlaylistMediasLoader
    {
        PlaylistMediasLoaderType LoaderType { get; }
        Task<bool>               LoadPlaylist(Playlist playlist);
    }
}