using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Nebula.Model;

namespace Nebula.Core
{
    public class PlaylistsManager
    {
        public ObservableCollection<Playlist> Playlists { get; } = new();

        public async Task LoadPlaylists()
        {
            List<Playlist> playlists = await NebulaClient.Database.GetPlaylists();
            foreach (Playlist playlist in playlists)
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
    }
}