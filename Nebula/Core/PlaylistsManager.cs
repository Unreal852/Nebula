using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using HandyControl.Controls;
using Nebula.Model;

namespace Nebula.Core
{
    public class PlaylistsManager
    {
        public PlaylistsManager()
        {
        }

        public ObservableCollection<Playlist> Playlists { get; } = new();

        public async Task LoadPlaylists()
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<Playlist> playlists = await NebulaClient.Database.GetPlaylists();
            sw.Stop();
            foreach (Playlist playlist in playlists)
                Playlists.Add(playlist);
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

        public async Task DeletePlaylist(Playlist playlist)
        {
            if (!Playlists.Contains(playlist))
                return;
            Playlists.Remove(playlist);
            await NebulaClient.Database.DeletePlaylist(playlist);
        }
    }
}