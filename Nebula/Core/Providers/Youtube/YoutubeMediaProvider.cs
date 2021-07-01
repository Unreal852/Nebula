using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nebula.Core.Playlists;
using Nebula.Model;
using YoutubeExplode;
using YoutubeExplode.Channels;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using Playlist = Nebula.Core.Playlists.Playlist;

namespace Nebula.Core.Providers.Youtube
{
    public class YoutubeMediaProvider : IMediasProvider, IPlaylistMediasLoader
    {
        public YoutubeClient            Youtube      { get; } = new();
        public string                   Url          => "https://www.youtube.com/";
        public string                   Name         => "Youtube";
        public ProviderType             ProviderType => ProviderType.Youtube;
        public PlaylistMediasLoaderType LoaderType   => PlaylistMediasLoaderType.Youtube;
        public string                   NameColorEx  => "#ff0000";

        public string GetArtistUrl(ArtistInfo artistInfo)
        {
            return $"https://www.youtube.com/channel/{artistInfo.ArtistId}";
        }

        public string GetMediaUrl(MediaInfo mediaInfo)
        {
            return $"https://www.youtube.com/watch?v={mediaInfo.MediaId}";
        }

        public string GetPlaylistUrl(Nebula.Core.Playlists.Playlist playlistInfo)
        {
            return $"https://www.youtube.com/playlist?list={playlistInfo.Info.PlaylistId}";
        }

        public async IAsyncEnumerable<MediaInfo> SearchMedias(string query, params object[] args)
        {
            await foreach (VideoSearchResult videoSearchResult in Search<VideoSearchResult>(query, args))
                yield return VideoToMediaInfo(videoSearchResult);
        }

        public async IAsyncEnumerable<Playlist> SearchPlaylists(string query, params object[] args)
        {
            await foreach (PlaylistSearchResult playlistResult in Search<PlaylistSearchResult>(query, args))
                yield return YoutubePlaylistToPlaylist(playlistResult);
        }

        public async IAsyncEnumerable<ArtistInfo> SearchArtists(string query, params object[] args)
        {
            await foreach (ChannelSearchResult channelResult in Search<ChannelSearchResult>(query, args))
                yield return ChannelToArtistInfo(channelResult);
        }

        public async IAsyncEnumerable<MediaInfo> GetArtistMedias(string query, params object[] args)
        {
            await foreach (PlaylistVideo video in Youtube.Channels.GetUploadsAsync(ChannelId.Parse(query)))
                yield return VideoToMediaInfo(video);
        }

        public async Task<MediaInfo> GetMediaInfo(string query, params object[] args)
        {
            return VideoToMediaInfo(await Youtube.Videos.GetAsync(VideoId.Parse(query)));
        }

        public async Task<ArtistInfo> GetArtistInfo(string query, params object[] args)
        {
            Channel channel = await Youtube.Channels.GetAsync(ChannelId.Parse(query));
            (string LowRes, string MediumRes, string HighRes) thumbnails = channel.Thumbnails.GetThumbnails();
            return new ArtistInfo(ProviderType, channel.Id.Value, channel.Title, thumbnails.LowRes, thumbnails.MediumRes,
                thumbnails.HighRes); // Logo is no longer available why?
        }

        public async Task<Playlist> GetPlaylist(string query, params object[] args)
        {
            YoutubeExplode.Playlists.Playlist youtubePlaylist = await Youtube.Playlists.GetAsync(query);
            Playlist playlist = YoutubePlaylistToPlaylist(youtubePlaylist);
            playlist.AutoSave = false;
            await foreach (PlaylistVideo video in Youtube.Playlists.GetVideosAsync(youtubePlaylist.Id))
                playlist.AddMedia(VideoToMediaInfo(video));
            playlist.IsLoaded = true;
            playlist.AutoSave = true;
            return playlist;
        }

        public async Task<Uri> GetAudioStreamUri(MediaInfo mediaInfo, params object[] args)
        {
            StreamManifest manifest = await Youtube.Videos.Streams.GetManifestAsync(VideoId.Parse(mediaInfo.MediaId));
            AudioOnlyStreamInfo streamInfo = manifest.GetAudioOnlyStreams().OrderByDescending(stream => stream.Bitrate.BitsPerSecond).First();
            return new Uri(streamInfo.Url);
        }

        public async Task<Uri> GetMuxedStreamUri(MediaInfo mediaInfo, params object[] args)
        {
            StreamManifest manifest = await Youtube.Videos.Streams.GetManifestAsync(VideoId.Parse(mediaInfo.MediaId));
            MuxedStreamInfo streamInfo = manifest.GetMuxedStreams().OrderByDescending(stream => stream.Bitrate.BitsPerSecond).First();
            return new Uri(streamInfo.Url);
        }

        public async Task<Uri> GetVideoStreamUri(MediaInfo mediaInfo, params object[] args)
        {
            StreamManifest manifest = await Youtube.Videos.Streams.GetManifestAsync(VideoId.Parse(mediaInfo.MediaId));
            IVideoStreamInfo streamInfo = manifest.GetVideoStreams().OrderByDescending(stream => stream.Bitrate.BitsPerSecond).First();
            return new Uri(streamInfo.Url);
        }

        public async IAsyncEnumerable<T> Search<T>(string query, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException($"{nameof(query)} is null or empty.");
            var totalElements = 0;
            int maxVideos = NebulaClient.Settings.General.MaxSearchElements;
            await foreach (Batch<ISearchResult> batch in Youtube.Search.GetResultBatchesAsync(query))
            {
                foreach (ISearchResult result in batch.Items)
                {
                    if (totalElements >= maxVideos)
                        break;
                    if (result is T element)
                    {
                        totalElements++;
                        yield return element;
                    }
                }

                if (totalElements >= maxVideos)
                    break;
            }
        }

        private MediaInfo VideoToMediaInfo(IVideo video)
        {
            (string LowRes, string MediumRes, string HighRes) thumbnails = video.Thumbnails.GetThumbnails();
            return new MediaInfo(ProviderType, video.Id.Value, video.Author.ChannelId.Value,
                video.Title, video.Author.Title, thumbnails.LowRes, thumbnails.MediumRes, thumbnails.HighRes, video.Duration ?? TimeSpan.Zero);
        }

        private ArtistInfo ChannelToArtistInfo(IChannel channel)
        {
            (string lowRes, string mediumRes, string highRes) = channel.Thumbnails.GetThumbnails();
            return new ArtistInfo(ProviderType, channel.Id.Value, channel.Title, lowRes, mediumRes, highRes);
        }

        private Playlist YoutubePlaylistToPlaylist(IPlaylist youtubePlaylist)
        {
            (string lowRes, string mediumRes, string highRes) = youtubePlaylist.Thumbnails.GetThumbnails();
            var playlistInfo = new PlaylistInfo(LoaderType, youtubePlaylist.Id.Value, youtubePlaylist.Author?.ChannelId.Value,
                youtubePlaylist.Title, youtubePlaylist.Author?.Title, "", lowRes, mediumRes, highRes);
            return new Playlist(playlistInfo);
        }

        public Task<bool> LoadPlaylist(Playlist playlist)
        {
            throw new NotImplementedException();
        }
    }
}