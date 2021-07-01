using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HandyControl.Controls;
using Nebula.Core.Playlists;
using Nebula.Core.Settings;
using Nebula.Model;
using SharpToolbox.Safes;
using SQLite;

namespace Nebula.Core.Database
{
    public class NebulaDatabase
    {
        private const string SelectPlaylistMediasOrderQuery = "SELECT * FROM PlaylistsMedias WHERE PlaylistIndex=? ORDER BY MediaOrder ASC";
        private const string SelectMediasFromMediaIdQuery   = "SELECT * FROM Medias WHERE MediaId=?";
        private const string UpdatePlaylistMediaQuery       = "UPDATE PlaylistsMedias SET IsActive=? WHERE PlaylistIndex=? AND MediaId=?";
        private const string InsertPlaylistMediasQuery      = "INSERT INTO PlaylistsMedias (PlaylistIndex, MediaId, MediaOrder, IsActive) VALUES(?,?,?,?)";
        private const string DeletePlaylistsMediaQuery      = "DELETE FROM PlaylistsMedias WHERE PlaylistIndex=? AND MediaId=?";
        private const string DeletePlaylistsMediasQuery     = "DELETE FROM PlaylistsMedias WHERE PlaylistIndex=?";
        private const string DeletePlaylistQuery            = "DELETE FROM Playlists WHERE PlaylistIndex=?";

        private const string DeleteUnusedMediasQuery =
            "DELETE FROM Medias WHERE MediaId IN (SELECT Medias.MediaId FROM Medias LEFT JOIN PlaylistsMedias ON Medias.MediaId=PlaylistsMedias.MediaId WHERE PlaylistsMedias.MediaId IS NULL)";

        public NebulaDatabase()
        {
            NebulaClient.Playlists.RegisterLoader(new DatabasePlaylistMediasLoader());
            LoadDatabase();
        }

        /// <summary>
        ///     The Database Connection
        /// </summary>
        private SQLiteAsyncConnection Database { get; set; }

        /// <summary>
        ///     Initialize and load the database
        /// </summary>
        private async void LoadDatabase()
        {
            string databaseFilePath = Path.Combine(AppSettings.SettingsDirectory.FullName, AppSettings.PlaylistDatabaseFileName);
            Database = new SQLiteAsyncConnection(databaseFilePath);
            SafeResult result = await Safe.TryAsync(async () => await Database.CreateTablesAsync<MediaInfo, ArtistInfo, PlaylistInfo, PlaylistMediaInfo>());
            if (!result.IsSuccess)
            {
                Growl.Error($"Resetting Database because it failed to load.{Environment.NewLine}{result.Exception.Message}");
                await Database.CloseAsync();
                if (File.Exists(databaseFilePath))
                    File.Delete(databaseFilePath);
                LoadDatabase();
                return;
            }

            Database.Tracer += OnReceiveTrace;
#if DEBUG
            Database.Trace = true;
#endif
            await Vacuum();
            await NebulaClient.Playlists.LoadPlaylists();
        }

        public async Task Vacuum()
        {
            await Database.ExecuteAsync("VACUUM");
        }

        /// <summary>
        ///     Get all playlists
        /// </summary>
        /// <returns>
        ///     <see cref="IAsyncEnumerable{T}" />
        /// </returns>
        public async IAsyncEnumerable<Playlist> GetPlaylists()
        {
            List<PlaylistInfo> playlistInfos = await Database.Table<PlaylistInfo>().ToListAsync();
            foreach (PlaylistInfo playlistInfo in playlistInfos)
                yield return new Playlist(playlistInfo);
        }

        /// <summary>
        ///     Get all <see cref="MediaInfo" /> for the specified <see cref="Playlist" />
        /// </summary>
        /// <param name="playlist">Playlist</param>
        /// <returns>
        ///     <see cref="List{T}" />
        /// </returns>
        public async Task<List<MediaInfo>> GetPlaylistMedias(Playlist playlist)
        {
            if (playlist == null)
                return new List<MediaInfo>();
            List<PlaylistMediaInfo> playlistMediaInfos =
                await Database.QueryAsync<PlaylistMediaInfo>(SelectPlaylistMediasOrderQuery, playlist.Info.PlaylistIndex);
            var medias = new List<MediaInfo>(playlistMediaInfos.Count);
            foreach (PlaylistMediaInfo pInfo in playlistMediaInfos)
            {
                MediaInfo mediaInfo = (await Database.QueryAsync<MediaInfo>(SelectMediasFromMediaIdQuery, pInfo.MediaId)).FirstOrDefault();
                pInfo.ApplyTo(mediaInfo);
                medias.Add(mediaInfo);
            }

            return medias;
        }

        /// <summary>
        ///     Update playlist media
        /// </summary>
        /// <param name="playlist">The playlist to update</param>
        /// <param name="mediaInfo">The media to update</param>
        public async Task UpdatePlaylistMedia(Playlist playlist, MediaInfo mediaInfo)
        {
            if (playlist == null || mediaInfo == null)
                return;
            await Database.ExecuteAsync(UpdatePlaylistMediaQuery, mediaInfo.IsActive, playlist.Info.PlaylistIndex, mediaInfo.MediaId);
        }

        public async Task InsertWholePlaylist(Playlist playlist)
        {
            if (playlist == null)
                return;
            await Database.RunInTransactionAsync(trans =>
            {
                trans.Insert(playlist.Info);
                if (playlist.MediasCount <= 0)
                    return;
                var index = 0;
                trans.InsertAll(playlist.Medias, "OR REPLACE");
                foreach (MediaInfo mediaInfo in playlist.Medias)
                    trans.Execute(InsertPlaylistMediasQuery, playlist.Info.PlaylistIndex, mediaInfo.MediaId, index++, true);
            });
        }

        public async Task InsertPlaylistMedia(Playlist playlist, MediaInfo mediaInfo, int order = -1)
        {
            if (playlist == null || mediaInfo == null)
                return;
            await Database.RunInTransactionAsync(trans =>
            {
                trans.InsertOrReplace(playlist.Info);
                trans.InsertOrReplace(mediaInfo);
                trans.Execute(InsertPlaylistMediasQuery,
                    playlist.Info.PlaylistIndex, mediaInfo.MediaId, order < 0 ? playlist.Medias.IndexOf(mediaInfo) : order, true);
            });
        }

        public async Task RemovePlaylistMedia(Playlist playlist, MediaInfo mediaInfo)
        {
            if (playlist == null || mediaInfo == null)
                return;
            await Database.RunInTransactionAsync(trans =>
            {
                trans.Execute(DeletePlaylistsMediaQuery, playlist.Info.PlaylistIndex, mediaInfo.MediaId);
                trans.Execute(DeleteUnusedMediasQuery);
            });
        }

        public async Task DeletePlaylist(Playlist playlist)
        {
            if (playlist == null)
                return;
            await Database.RunInTransactionAsync(trans =>
            {
                trans.Execute(DeletePlaylistQuery, playlist.Info.PlaylistIndex);
                trans.Execute(DeletePlaylistsMediasQuery, playlist.Info.PlaylistIndex);
                trans.Execute(DeleteUnusedMediasQuery);
            });
        }

        private void OnReceiveTrace(string obj)
        {
            NebulaClient.Logger.Log(obj);
        }
    }
}