using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nebula.Core.Providers.Youtube.Extensions;
using Nebula.Media;
using Nebula.Model;
using YoutubeExplode;
using YoutubeExplode.Channels;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using Playlist = Nebula.Model.Playlist;

namespace Nebula.Core.Providers.Youtube
{
    public class YoutubeMediaProvider : IMediasProvider
    {
        private const int MaxVideos = 50;

        public YoutubeClient Youtube { get; } = new();

        public string Url         { get; } = "https://www.youtube.com/";
        public string Name        { get; } = "Youtube";
        public string NameColorEx { get; } = "#ff0000";

        public async IAsyncEnumerable<T> Search<T>(string query, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException($"{nameof(query)} is null or empty.");
            var totalElements = 0;
            await foreach (Batch<ISearchResult> batch in Youtube.Search.GetResultBatchesAsync(query))
            {
                foreach (ISearchResult result in batch.Items)
                {
                    if (totalElements >= MaxVideos)
                        break;
                    if (result is T element)
                    {
                        totalElements++;
                        yield return element;
                    }
                }

                if (totalElements >= MaxVideos)
                    break;
            }
        }

        public async IAsyncEnumerable<MediaInfo> SearchMedias(string query, params object[] args)
        {
            await foreach (VideoSearchResult videoSearchResult in Search<VideoSearchResult>(query, args))
                yield return VideoToMediaInfo(videoSearchResult);

            /*
            await foreach (VideoSearchResult video in Youtube.Search.GetVideosAsync(query))
                yield return new YoutubeMediaInfo(video); */
        }

        public async IAsyncEnumerable<Playlist> SearchPlaylists(string query, params object[] args)
        {
            await foreach (PlaylistSearchResult playlistResult in Search<PlaylistSearchResult>(query, args))
                yield return YoutubePlaylistToPlaylist(playlistResult);
        }

        public IAsyncEnumerable<ArtistInfo> SearchArtists(string query, params object[] args)
        {
            throw new NotImplementedException();
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

        public async Task<IArtistInfo> GetArtistInfo(string query, params object[] args)
        {
            Channel channel = await Youtube.Channels.GetAsync(ChannelId.Parse(query));
            (string LowRes, string MediumRes, string HighRes) thumbnails = channel.Thumbnails.GetThumbnails();
            return new ArtistInfo(channel.Id, channel.Title, channel.Url, thumbnails.LowRes, thumbnails.MediumRes,
                thumbnails.HighRes); // Logo is no longer available why?
        }

        public async Task<Playlist> GetPlaylist(string query, params object[] args)
        {
            YoutubeExplode.Playlists.Playlist youtubePlaylist = await Youtube.Playlists.GetAsync(query);
            (string LowRes, string MediumRes, string HighRes) thumbnails = youtubePlaylist.Thumbnails.GetThumbnails();
            var playlist = new Playlist
            {
                Name = youtubePlaylist.Title,
                Description = youtubePlaylist.Description,
                Author = youtubePlaylist.Author?.Title ?? "Unknown",
                Url = youtubePlaylist.Url,
                LowResThumbnail = thumbnails.LowRes,
                MediumResThumbnail = thumbnails.MediumRes,
                HighResThumbnail = thumbnails.HighRes,
                AutoSave = false,
                ProviderName = Name
            };
            await foreach (PlaylistVideo video in Youtube.Playlists.GetVideosAsync(youtubePlaylist.Id))
                playlist.AddMedia(VideoToMediaInfo(video));
            playlist.IsLoaded = true;
            playlist.AutoSave = true;
            return playlist;
        }

        public async Task<Uri> GetAudioStreamUri(IMediaInfo mediaInfo, params object[] args)
        {
            StreamManifest manifest = await Youtube.Videos.Streams.GetManifestAsync(VideoId.Parse(mediaInfo.Id));
            AudioOnlyStreamInfo streamInfo = manifest.GetAudioOnlyStreams().OrderByDescending(stream => stream.Bitrate.BitsPerSecond).First();
            return new Uri(streamInfo.Url);
        }

        public async Task<Uri> GetMuxedStreamUri(IMediaInfo mediaInfo, params object[] args)
        {
            StreamManifest manifest = await Youtube.Videos.Streams.GetManifestAsync(VideoId.Parse(mediaInfo.Id));
            MuxedStreamInfo streamInfo = manifest.GetMuxedStreams().OrderByDescending(stream => stream.Bitrate.BitsPerSecond).First();
            return new Uri(streamInfo.Url);
        }

        public async Task<Uri> GetVideoStreamUri(IMediaInfo mediaInfo, params object[] args)
        {
            StreamManifest manifest = await Youtube.Videos.Streams.GetManifestAsync(VideoId.Parse(mediaInfo.Id));
            IVideoStreamInfo streamInfo = manifest.GetVideoStreams().OrderByDescending(stream => stream.Bitrate.BitsPerSecond).First();
            return new Uri(streamInfo.Url);
        }

        private MediaInfo VideoToMediaInfo(IVideo video)
        {
            (string LowRes, string MediumRes, string HighRes) thumbnails = video.Thumbnails.GetThumbnails();
            return new MediaInfo(video.Id.Value, video.Author.ChannelId.Value,
                video.Title, video.Author.Title, "", Name,
                thumbnails.LowRes, thumbnails.MediumRes, thumbnails.HighRes,
                video.Duration ?? TimeSpan.Zero, DateTime.MinValue);
        }

        private Playlist YoutubePlaylistToPlaylist(PlaylistSearchResult playlistSearchResult)
        {
            (string LowRes, string MediumRes, string HighRes) thumbnails = playlistSearchResult.Thumbnails.GetThumbnails();
            var playlist = new Playlist
            {
                Name = playlistSearchResult.Title,
                Author = playlistSearchResult.Author?.Title ?? "Unknown",
                Url = playlistSearchResult.Url,
                LowResThumbnail = thumbnails.LowRes,
                MediumResThumbnail = thumbnails.MediumRes,
                HighResThumbnail = thumbnails.HighRes,
                AutoSave = false,
                ProviderName = Name
            };
            return playlist;
        }
    }
}