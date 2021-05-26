using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using HandyControl.Controls;
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

        public ObservableCollection<Playlist> Playlists { get; } = new();

        private async void LoadPlaylists()
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<Playlist> playlists = await NebulaClient.Database.GetAllPlaylists();
            foreach (Playlist playlist in playlists)
                Playlists.Add(playlist);
            sw.Stop();
            Growl.Info($"Loaded playlists in {sw.Elapsed.TotalMilliseconds}ms");
        }

        public async void AddPlaylist(Playlist playlist)
        {
            if (Playlists.Contains(playlist))
                return;
            Playlists.Add(playlist);
            await NebulaClient.Database.InsertWholePlaylist(playlist);
            Growl.Info("Imported and saved " + playlist.Name + " (" + playlist.MediasCount + ")");
        }
    }
}