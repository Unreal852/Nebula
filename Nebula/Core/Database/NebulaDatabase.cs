using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HandyControl.Controls;
using HandyControl.Data;
using Nebula.Media;
using Nebula.Model;
using SQLite;
using Playlist = Nebula.Model.Playlist;

namespace Nebula.Core.Database
{
    public class NebulaDatabase
    {
        private static readonly string DatabaseFile = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\userdata.db";

        public NebulaDatabase()
        {
            Database = new SQLiteAsyncConnection(DatabaseFile);
            Database.CreateTableAsync<ArtistInfo>();
            Database.CreateTableAsync<MediaInfo>();
            Database.CreateTableAsync<Playlist>();
            Database.ExecuteAsync(
                @"create table if not exists PlaylistsMedias (PlaylistId varchar(50) not null, MediaId varchar(50) not null, ""Order"" int, IsActive int)");
            Database.Tracer += OnReceiveTrace;
            Database.Trace = false;
        }

        public SQLiteAsyncConnection Database { get; }

        public async Task<List<MediaInfo>> GetAllMedias()                             => await Database.Table<MediaInfo>().ToListAsync();
        public async Task<IMediaInfo>      GetMediaById(string id)                    => await Database.Table<MediaInfo>().FirstAsync(media => media.Id == id);
        public async Task<IArtistInfo>     GetArtistById(string id)                   => await Database.Table<ArtistInfo>().FirstAsync(artist => artist.Id == id);
        public async Task                  InsertMedia(IMediaInfo mediaInfo)          => await Database.InsertOrReplaceAsync(mediaInfo);
        public async Task                  InsertOrReplacePlaylist(Playlist playlist) => await Database.InsertOrReplaceAsync(playlist);

        public async Task UpdatePlaylistMedia(Playlist playlist, MediaInfo mediaInfo)
        {
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
                foreach (MediaInfo mediaInfo in playlist.Medias.Elements)
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
            await Database.RunInTransactionAsync(async trans =>
            {
                trans.InsertOrReplace(playlist);
                trans.InsertOrReplace(mediaInfo);
                trans.Execute($"insert into PlaylistsMedias (PlaylistId, MediaId, \"Order\", IsActive) values(?,?,?,?)",
                    playlist.Id, mediaInfo.Id, order < 0 ? playlist.Medias.Elements.IndexOf(mediaInfo) : order, true);
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
                    playlist.Medias.SetElements(await GetPlaylistMedias(playlist));
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

        private void OnReceiveTrace(string obj)
        {
            GrowlInfo info = new GrowlInfo {Message = obj, Token = ""};
            Growl.Warning(info);
        }
    }
}