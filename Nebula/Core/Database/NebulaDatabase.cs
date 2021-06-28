using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HandyControl.Controls;
using HandyControl.Data;
using Nebula.Core.Settings;
using Nebula.Model;
using SQLite;

namespace Nebula.Core.Database
{
    public class NebulaDatabase
    {
        public NebulaDatabase()
        {
            Database = new SQLiteAsyncConnection(Path.Combine(AppSettings.SettingsDirectory.FullName, AppSettings.PlaylistDatabaseFileName));
            
            LoadDatabase();
        }

        /// <summary>
        /// The Database Connection
        /// </summary>
        private SQLiteAsyncConnection Database { get; }

        /// <summary>
        /// Initialize and load the database
        /// </summary>
        private async void LoadDatabase()
        {
            await Database.CreateTablesAsync<MediaInfo, ArtistInfo, Playlist, PlaylistMediaInfo>();
            Database.Tracer += OnReceiveTrace;
            Database.Trace = false;
            await Vacuum();
            await NebulaClient.Playlists.LoadPlaylists();
        }

        public async Task<List<Playlist>> GetPlaylists() => await Database.Table<Playlist>().ToListAsync();
        public async Task                 Vacuum()       => await Database.ExecuteAsync("VACUUM");


        /// <summary>
        /// Get all <see cref="MediaInfo"/> for the specified <see cref="Playlist"/>
        /// </summary>
        /// <param name="playlist">Playlist</param>
        /// <returns><see cref="List{T}"/></returns>
        public async Task<List<MediaInfo>> GetPlaylistMedias(Playlist playlist)
        {
            if (playlist == null)
                return new List<MediaInfo>();
            List<PlaylistMediaInfo> playlistMediaInfos =
                await Database.QueryAsync<PlaylistMediaInfo>($"SELECT * FROM PlaylistsMedias WHERE PlaylistId={playlist.PlaylistId} ORDER BY \"Order\" ASC");
            List<MediaInfo> medias = new List<MediaInfo>(playlistMediaInfos.Count);
            foreach (PlaylistMediaInfo pInfo in playlistMediaInfos)
            {
                MediaInfo mediaInfo = (await Database.QueryAsync<MediaInfo>("SELECT * FROM Medias WHERE Id=?", pInfo.MediaId)).FirstOrDefault();
                pInfo.ApplyTo(mediaInfo);
                medias.Add(mediaInfo);
            }

            return medias;
        }

        /// <summary>
        /// Create table if not exists into the database
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <param name="columns">The table columns</param>
        private async Task CreateTable(string tableName, params string[] columns)
        {
            string query = $"CREATE TABLE IF NOT EXISTS {tableName} ({string.Join(',', columns)})";
            await Database.ExecuteAsync(query);
        }

        public async Task UpdatePlaylistMedia(Playlist playlist, MediaInfo mediaInfo)
        {
            if (playlist == null || mediaInfo == null)
                return;
            await Database.ExecuteAsync("UPDATE PlaylistsMedias SET IsActive=? WHERE PlaylistId=? AND MediaId=?", mediaInfo.IsActive, playlist.PlaylistId, mediaInfo.MediaId);
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
                    trans.Execute("INSERT INTO PlaylistsMedias (PlaylistId, MediaId, \"Order\", IsActive) VALUES(?,?,?,?)",
                        playlist.PlaylistId, mediaInfo.MediaId, index++, true);
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
                trans.Execute("INSERT INTO PlaylistsMedias (PlaylistId, MediaId, \"Order\", IsActive) VALUES(?,?,?,?)",
                    playlist.PlaylistId, mediaInfo.MediaId, order < 0 ? playlist.Medias.IndexOf(mediaInfo) : order, true);
            });
        }

        public async Task RemovePlaylistMedia(Playlist playlist, MediaInfo mediaInfo)
        {
            if (playlist == null || mediaInfo == null)
                return;
            await Database.ExecuteAsync("DELETE FROM PlaylistsMedias WHERE PlaylistId=? AND MediaId=?", playlist.PlaylistId, mediaInfo.MediaId);
        }

        public async Task DeletePlaylist(Playlist playlist)
        {
            if (playlist == null)
                return;
            await Database.RunInTransactionAsync(trans =>
            {
                trans.Execute("DELETE FROM Playlists WHERE Id=?", playlist.PlaylistId);
                trans.Execute("DELETE FROM PlaylistsMedias WHERE PlaylistId=?", playlist.PlaylistId);
                trans.Execute(
                    "DELETE FROM Medias WHERE MediaId IN (SELECT Medias.MediaId FROM Medias LEFT JOIN PlaylistsMedias ON Medias.MediaId=PlaylistsMedias.MediaId WHERE PlaylistsMedias.MediaId IS NULL)");
            });
        }

        private void OnReceiveTrace(string obj)
        {
            GrowlInfo info = new GrowlInfo {Message = obj, Token = ""};
            Growl.Warning(info);
        }
    }
}