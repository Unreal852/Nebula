using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nebula.Media;
using Nebula.Model;
using YoutubeExplode;
using YoutubeExplode.Channels;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Nebula.Core.Providers.Youtube
{
    public class YoutubeMediaProvider : IMediasProvider
    {
        private const int MaxVideos = 50;

        public YoutubeMediaProvider()
        {
        }

        public string Url         { get; } = "https://www.youtube.com/";
        public string Name        { get; } = "Youtube";
        public string NameColorEx { get; } = "#ff0000";

        public YoutubeClient Youtube { get; } = new();

        public async IAsyncEnumerable<IMediaInfo> SearchMedias(string query, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException($"{nameof(query)} is null or empty.");
            int totalVideos = 0;
            await foreach (Batch<ISearchResult> batch in Youtube.Search.GetResultBatchesAsync(query))
            {
                foreach (ISearchResult result in batch.Items)
                {
                    if (totalVideos >= MaxVideos)
                        break;
                    if (result is VideoSearchResult video)
                    {
                        totalVideos++;
                        yield return VideoToMediaInfo(video);
                    }
                }

                if (totalVideos >= MaxVideos)
                    break;
            }

            /*
            await foreach (VideoSearchResult video in Youtube.Search.GetVideosAsync(query))
                yield return new YoutubeMediaInfo(video); */
        }

        public async IAsyncEnumerable<IMediaInfo> GetArtistMedias(string query, params object[] args)
        {
            await foreach (PlaylistVideo video in Youtube.Channels.GetUploadsAsync(ChannelId.Parse(query)))
                yield return new YoutubeMediaInfo(video);
        }

        public async Task<IMediaInfo> GetMediaInfo(string query, params object[] args)
        {
            return new YoutubeMediaInfo(await Youtube.Videos.GetAsync(VideoId.Parse(query)));
        }

        public async Task<IArtistInfo> GetArtistInfo(string query, params object[] args)
        {
            Channel channel = await Youtube.Channels.GetAsync(ChannelId.Parse(query));
            return new YoutubeArtistInfo(channel.Id, channel.Title, channel.Url, channel.Thumbnails.First().Url); // Logo is no longer available why?
        }

        public async Task<Media.IPlaylist> GetPlaylist(string query, params object[] args)
        {
            YoutubeExplode.Playlists.Playlist youtubePlaylist = await Youtube.Playlists.GetAsync(query);
            Model.Playlist playlist = new Model.Playlist()
            {
                Name = youtubePlaylist.Title, Description = youtubePlaylist.Description, Author = youtubePlaylist.Author?.Title ?? "Unknown",
                Thumbnail = new Uri(youtubePlaylist.Thumbnails.OrderBy(t => t.Resolution.Area).First().Url), AutoSave = false
            };
            await foreach (PlaylistVideo video in Youtube.Playlists.GetVideosAsync(youtubePlaylist.Id))
                playlist.AddMedia(VideoToMediaInfo(video));
            playlist.AutoSave = true;
            return playlist;
        }

        public async Task<Model.Playlist> GetPlaylistt(string query, params object[] args)
        {
            YoutubeExplode.Playlists.Playlist youtubePlaylist = await Youtube.Playlists.GetAsync(query);
            Model.Playlist playlist = new Model.Playlist()
            {
                Name = youtubePlaylist.Title, Description = youtubePlaylist.Description, Author = youtubePlaylist.Author?.Title ?? "Unknown",
                Thumbnail = new Uri(youtubePlaylist.Thumbnails.OrderBy(t => t.Resolution.Area).First().Url), AutoSave = false
            };
            await foreach (PlaylistVideo video in Youtube.Playlists.GetVideosAsync(youtubePlaylist.Id))
                playlist.AddMedia(VideoToMediaInfo(video));
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

        private static MediaInfo VideoToMediaInfo(IVideo video)
        {
            Thumbnail[] thumbnails = video.Thumbnails.OrderBy(thumbRes => thumbRes.Resolution.Area).ToArray();
            string lowResThumbnailUrl = thumbnails[0].Url;
            string highResThumbnailUrl = thumbnails[^1].Url;
            string mediumResThumbnailUrl = thumbnails.Length >= 2 ? thumbnails[thumbnails.Length / 2 - 1].Url : highResThumbnailUrl;
            return new MediaInfo(video.Id.Value, video.Author.ChannelId.Value,
                video.Title, video.Author.Title, "",
                lowResThumbnailUrl, mediumResThumbnailUrl, highResThumbnailUrl,
                video.Duration ?? TimeSpan.Zero, DateTime.MinValue);
        }
    }
}