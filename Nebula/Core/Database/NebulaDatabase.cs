using System;
using System.Collections.Generic;
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
                @"create table if not exists PlaylistsMedias (PlaylistId varchar(50) not null, SongId varchar(50) not null, ""Order"" int, IsActive int)");
            Database.Tracer += OnReceiveTrace;
            Database.Trace = true;
        }

        public SQLiteAsyncConnection Database { get; }

        public async Task<List<MediaInfo>> GetAllMedias()                     => await Database.Table<MediaInfo>().ToListAsync();
        public async Task<List<Playlist>>  GetAllPlaylists()                  => await Database.Table<Playlist>().ToListAsync();
        public async Task<IMediaInfo>      GetMediaById(string id)            => await Database.Table<MediaInfo>().FirstAsync(media => media.Id == id);
        public async Task<IArtistInfo>     GetArtistById(string id)           => await Database.Table<ArtistInfo>().FirstAsync(artist => artist.Id == id);
        public async Task                  InsertMedia(IMediaInfo mediaInfo)  => await Database.InsertOrReplaceAsync(mediaInfo);
        public async Task                  InsertPlaylist(IPlaylist playlist) => await Database.InsertAsync(playlist);

        public async IAsyncEnumerable<IMediaInfo> GetPlaylistMedias(IPlaylist playlist)
        {
            
        }

        public async Task InsertPlaylistMedia(IPlaylist playlist, IMediaInfo mediaInfo, int order)
        {
            if (playlist == null || mediaInfo == null)
                return;
            await Database.RunInTransactionAsync(async trans =>
            {
                await Database.InsertOrReplaceAsync(playlist);
                await Database.InsertOrReplaceAsync(mediaInfo);
                await Database.ExecuteAsync($"insert into PlaylistsMedias (PlaylistId, SongId, \"Order\", IsActive) values(?,?,?,?)",
                    playlist.Id, mediaInfo.Id, order, true);
            });
        }

        private void OnReceiveTrace(string obj)
        {
            GrowlInfo info = new GrowlInfo {Message = obj, Token = ""};
            Growl.Warning(info);
        }
    }

    public class PlaylistMediaInfo
    {
        public string PlaylistId { get; set; }
        public string MediaId    { get; set; }
        public int    Order      { get; set; }
        public bool   IsActive   { get; set; }
    }
}