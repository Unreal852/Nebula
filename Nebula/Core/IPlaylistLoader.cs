using System.Threading.Tasks;
using Nebula.Model;

namespace Nebula.Core
{
    public interface IPlaylistLoader
    {
        Task<bool> LoadPlaylist(Playlist playlist);
    }
}