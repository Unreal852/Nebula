using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Nebula.Core.Playlists
{
    public class PlaylistsManager
    {
        public PlaylistsManager()
        {
        }

        public  ObservableCollection<Playlist>                              Playlists { get; } = new();
        private Dictionary<PlaylistMediasLoaderType, IPlaylistMediasLoader> Loaders   { get; } = new();

        public async Task LoadPlaylists()
        {
            Playlists.Clear();
            await foreach(Playlist playlist in NebulaClient.Database.GetPlaylists())
                Playlists.Add(playlist);
        }

        public async void AddPlaylist(Playlist playlist)
        {
            if (Playlists.Contains(playlist))
                return;
            Playlists.Add(playlist);
            await NebulaClient.Database.InsertWholePlaylist(playlist);
        }

        public async Task DeletePlaylist(Playlist playlist)
        {
            if (!Playlists.Contains(playlist))
                return;
            Playlists.Remove(playlist);
            await NebulaClient.Database.DeletePlaylist(playlist);
        }

        public void RegisterLoader(IPlaylistMediasLoader loaderType)
        {
            if (Loaders.ContainsKey(loaderType.LoaderType))
                return;
            Loaders.Add(loaderType.LoaderType, loaderType);
        }

        public IPlaylistMediasLoader GetLoader(PlaylistMediasLoaderType loaderType)
        {
            return Loaders.ContainsKey(loaderType) ? Loaders[loaderType] : new DefaultPlaylistMediasLoader();
        }
    }
}