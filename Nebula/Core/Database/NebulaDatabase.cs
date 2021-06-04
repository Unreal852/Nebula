using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HandyControl.Controls;
using HandyControl.Data;
using Nebula.Model;
using SQLite;
using Playlist = Nebula.Model.Playlist;

namespace Nebula.Core.Database
{
    public class NebulaDatabase
    {
        private static readonly string DatabaseFile     = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\userdata.db";
        private const           string PLAYLISTS_MEDIAS = "PlaylistsMedias";

        public NebulaDatabase()
        {
            Database = new SQLiteAsyncConnection(DatabaseFile);
            LoadDatabase();
        }

        public SQLiteAsyncConnection Database { get; }

        private async void LoadDatabase()
        {
            await Database.CreateTablesAsync<MediaInfo, ArtistInfo, Playlist, PlaylistMediaInfo>();
            Database.Tracer += OnReceiveTrace;
            Database.Trace = false;
            await NebulaClient.Playlists.LoadPlaylists();
        }

        public async Task<List<Playlist>> GetPlaylists() => await Database.Table<Playlist>().ToListAsync();

        private async Task CreateTable(string tableName, params string[] columns)
        {
            string query = $"CREATE TABLE IF NOT EXISTS {tableName} ({string.Join(',', columns)})";
            await Database.ExecuteAsync(query);
        }

        public async Task UpdatePlaylistMedia(Playlist playlist, MediaInfo mediaInfo)
        {
            if (playlist == null || mediaInfo == null)
                return;
            await Database.ExecuteAsync($"UPDATE PlaylistsMedias SET IsActive=? WHERE PlaylistId=? AND MediaId=?", mediaInfo.IsActive, playlist.Id, mediaInfo.Id);
        }

        public async Task InsertWholePlaylist(Playlist playlist)
        {
            if (playlist == null)
                return;
            await Database.RunInTransactionAsync(trans =>
            {
                trans.Insert(playlist);
                if (playlist.MediasCount <= 0)
                    return;
                int index = 0;
                foreach (MediaInfo mediaInfo in playlist.Medias)
                {
                    trans.InsertOrReplace(mediaInfo);
                    trans.Execute($"insert into PlaylistsMedias (PlaylistId, MediaId, \"Order\", IsActive) values(?,?,?,?)",
                        playlist.Id, mediaInfo.Id, index++, true);
                }
            });
        }

        public async Task InsertPlaylistMedia(Playlist playlist, MediaInfo mediaInfo, int order = -1)
        {
            if (playlist == null || mediaInfo == null)
                return;
            await Database.RunInTransactionAsync(trans =>
            {
                trans.InsertOrReplace(playlist);
                trans.InsertOrReplace(mediaInfo);
                trans.Execute($"insert into PlaylistsMedias (PlaylistId, MediaId, \"Order\", IsActive) values(?,?,?,?)",
                    playlist.Id, mediaInfo.Id, order < 0 ? playlist.Medias.IndexOf(mediaInfo) : order, true);
            });
        }

        public async Task RemovePlaylistMedia(Playlist playlist, MediaInfo mediaInfo)
        {
            if (playlist == null || mediaInfo == null)
                return;
            await Database.ExecuteAsync("DELETE FROM PlaylistsMedias WHERE PlaylistId=? AND MediaId=?", playlist.Id, mediaInfo.Id);
        }

        public async Task<List<Playlist>> GetAllPlaylists(bool loadMedias = true)
        {
            List<Playlist> playlists = await Database.Table<Playlist>().ToListAsync();
            if (loadMedias)
            {
                foreach (Playlist playlist in playlists)
                {
                    foreach (MediaInfo mediaInfo in await GetPlaylistMedias(playlist))
                        playlist.Medias.Add(mediaInfo);
                }
            }

            return playlists;
        }

        public async Task<List<MediaInfo>> GetPlaylistMedias(Playlist playlist)
        {
            List<PlaylistMediaInfo> playlistMediaInfos =
                await Database.QueryAsync<PlaylistMediaInfo>($"SELECT * FROM PlaylistsMedias WHERE PlaylistId={playlist.Id} ORDER BY \"Order\" ASC");
            List<MediaInfo> medias = new List<MediaInfo>(playlistMediaInfos.Count);
            foreach (PlaylistMediaInfo pInfo in playlistMediaInfos)
            {
                MediaInfo mediaInfo = await Database.Table<MediaInfo>().FirstAsync(m => m.Id == pInfo.MediaId);
                pInfo.ApplyTo(mediaInfo);
                medias.Add(mediaInfo);
            }

            return medias;
        }

        public async Task DeletePlaylist(Playlist playlist)
        {
            if (playlist == null)
                return;
            Database.Trace = false;
            await Database.RunInTransactionAsync(trans =>
            {
                trans.Execute("DELETE FROM Playlists WHERE Id=?", playlist.Id);
                trans.Execute("DELETE FROM PlaylistsMedias WHERE PlaylistId=?", playlist.Id);
                trans.Execute(
                    "DELETE FROM Medias WHERE Id IN (SELECT Medias.Id FROM Medias LEFT JOIN PlaylistsMedias ON Medias.Id=PlaylistsMedias.MediaId WHERE PlaylistsMedias.MediaId IS NULL)");
            });
        }

        private void OnReceiveTrace(string obj)
        {
            GrowlInfo info = new GrowlInfo {Message = obj, Token = ""};
            Growl.Warning(info);
        }
    }
}