using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nebula.Core.Providers.Youtube.Extensions;
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
        public YoutubeClient Youtube     { get; } = new();
        public string        Url         => "https://www.youtube.com/";
        public string        Name        => "Youtube";
        public string        NameColorEx => "#ff0000";

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

        public async IAsyncEnumerable<ArtistInfo> SearchArtists(string query, params object[] args)
        {
            await foreach (ChannelSearchResult channelSearchResult in Search<ChannelSearchResult>(query, args))
                yield return ChannelToArtist(channelSearchResult);
        }

        public async IAsyncEnumerable<MediaInfo> GetArtistMedias(ArtistInfo artistInfo, params object[] args)
        {
            await foreach (PlaylistVideo video in Youtube.Channels.GetUploadsAsync(artistInfo.AuthorId))
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
            return new ArtistInfo(channel.Id, channel.Title, channel.Url, thumbnails.LowRes, thumbnails.MediumRes,
                thumbnails.HighRes); // Logo is no longer available why?
        }

        public async Task<Playlist> GetPlaylist(string query, params object[] args)
        {
            YoutubeExplode.Playlists.Playlist youtubePlaylist = await Youtube.Playlists.GetAsync(query);
            (string LowRes, string MediumRes, string HighRes) thumbnails = youtubePlaylist.Thumbnails.GetThumbnails();
            Playlist playlist = YoutubePlaylistToPlaylist(youtubePlaylist);
            playlist.Description = youtubePlaylist.Description;
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
            (string lowRes, string mediumRes, string highRes) = video.Thumbnails.GetThumbnails();
            return new MediaInfo(video.Id.Value, video.Author.ChannelId.Value,
                video.Title, video.Author.Title, Name,
                lowRes, mediumRes, highRes, video.Duration ?? TimeSpan.Zero);
        }

        private ArtistInfo ChannelToArtist(IChannel channel)
        {
            (string lowRes, string mediumRes, string highRes) = channel.Thumbnails.GetThumbnails();
            return new ArtistInfo(channel.Id, Name, channel.Title, lowRes, mediumRes, highRes);
        }

        private Playlist YoutubePlaylistToPlaylist(IPlaylist playlistSearchResult)
        {
            (string lowRes, string mediumRes, string highRes) = playlistSearchResult.Thumbnails.GetThumbnails();
            var playlist = new Playlist
            {
                Name = playlistSearchResult.Title,
                Author = playlistSearchResult.Author?.Title ?? "Unknown",
                LowResThumbnail = lowRes,
                MediumResThumbnail = mediumRes,
                HighResThumbnail = highRes,
                AutoSave = false,
                ProviderName = Name
            };
            return playlist;
        }
    }
}