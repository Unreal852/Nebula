using System.Collections.Generic;
using System.Collections.ObjectModel;
using Nebula.Media;
using Nebula.Model;

namespace Nebula.Core
{
    public class PlaylistsManager
    {
        public PlaylistsManager()
        {
            LoadPlaylists();
        }

        public ObservableCollection<IPlaylist> Playlists       { get; } = new();
        public Dictionary<string, IMediaInfo>  PlaylistsMedias { get; } = new();

        private async void LoadPlaylists()
        {

            await foreach (IPlaylist playlist in NebulaClient.Database.GetAllPlaylists())
                Playlists.Add(playlist);
        }

        public async void AddPlaylist(IPlaylist playlist)
        {
            if (Playlists.Contains(playlist))
                return;
            Playlists.Add(playlist);
            await NebulaClient.Database.InsertPlaylist(playlist);
        }
    }
}