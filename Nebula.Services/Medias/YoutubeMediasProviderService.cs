using Nebula.Common.Extensions;
using Nebula.Common.Medias;
using Nebula.Services.Abstractions;
using Serilog;
using SerilogTimings.Extensions;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using Playlist = Nebula.Common.Playlist.Playlist;

namespace Nebula.Services.Medias;

public sealed class YoutubeMediasProviderService : IMediasProviderService
{
    private readonly ILogger _logger;

    public YoutubeMediasProviderService(ILogger logger)
    {
        _logger = logger.WithPrefix(nameof(YoutubeMediasProviderService));
    }

    private YoutubeClient Client { get; } = new();

    public Guid   ServiceId   { get; } = new("65381A09-4DB9-42A7-9DED-824E7469647A");
    public string ServiceName { get; } = "Youtube";

    public string ServiceIcon { get; }
        = "M380,70H110C49.346,70,0,119.346,0,180v130c0,60.654,49.346,110,110,110h270c60.654,0,110-49.346,110-110V180     C490,119.346,440.654,70,380,70z M470,310c0,49.626-40.374,90-90,90H110c-49.626,0-90-40.374-90-90V180c0-49.626,40.374-90,90-90     h270c49.626,0,90,40.374,90,90V310z M323.846,235.769l-120-50c-3.085-1.286-6.611-0.945-9.393,0.911c-2.782,1.854-4.453,4.977-4.453,8.32v100     c0,3.344,1.671,6.466,4.453,8.32c1.667,1.112,3.601,1.68,5.548,1.68c1.301,0,2.608-0.254,3.845-0.769l120-50     c3.727-1.553,6.154-5.194,6.154-9.231C330,240.963,327.572,237.322,323.846,235.769z M210,280v-70l84,35L210,280z";

    public string ServiceIconColor { get; } = "Red";

    public async Task<IMediaInfo?> GetMediaAsync(string url)
    {
        var videoId = VideoId.TryParse(url);
        if (videoId == null)
        {
            _logger.Warning("Invalid video id '{VideoId}'", url);
            return MediaInfo.Empty;
        }

        var media = default(MediaInfo);

        using var op = _logger.BeginOperation("Fetching youtube media with id '{MediaId}'", videoId.Value.Value);
        try
        {
            var video = await Client.Videos.GetAsync(videoId.Value);
            var streamManifest = await Client.Videos.Streams.GetManifestAsync(videoId.Value);
            var stream = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            media = new MediaInfo
            {
                    Id = video.Id.Value,
                    Title = video.Title,
                    Author = video.Author.ChannelTitle,
                    Duration = video.Duration?.TotalSeconds ?? 0,
                    StreamUri = stream.Url
            };
            op.Complete();
        }
        catch (Exception ex)
        {
            op.SetException(ex).Abandon();
        }

        return media;
    }

    public async Task<Playlist?> GetPlaylistAsync(string url)
    {
        var playlistId = PlaylistId.TryParse(url);
        if (playlistId == null)
        {
            _logger.Warning("Invalid playlist id '{PlaylistId}'", url);
            return default;
        }

        using var op
                = _logger.BeginOperation("Fetching youtube playlist with id '{PlaylistId}'", playlistId.Value.Value);
        try
        {
            var playlist = await Client.Playlists.GetAsync(playlistId.Value.Value);
            IReadOnlyList<PlaylistVideo> videos = await Client.Playlists.GetVideosAsync(playlistId.Value);
            var userPlaylist = new Playlist { Name = playlist.Title, Author = playlist.Author?.ChannelTitle };
            foreach (var playlistVideo in videos)
                userPlaylist.Medias.Add(MediaFromVideo(playlistVideo));

            op.Complete();
            return userPlaylist;
        }
        catch (Exception ex)
        {
            op.SetException(ex).Abandon();
            return default;
        }
    }

    public async IAsyncEnumerable<IMediaInfo> SearchMediasAsync(string searchQuery, int maxResults = 20)
    {
        if (!string.IsNullOrWhiteSpace(searchQuery))
            using (_logger.TimeOperation("Searching on youtube... Query: '{Query}' MaxResults: '{MaxResults}'",
                           searchQuery, maxResults))
            {
                var found = 0;
                await foreach (var video in Client.Search.GetVideosAsync(searchQuery))
                {
                    if (found >= maxResults)
                        break;
                    found++;
                    yield return MediaFromVideo(video);
                }
            }
        else
            _logger.Warning("Search query is null or empty");
    }

    private MediaInfo MediaFromVideo(IVideo video)
    {
        return new MediaInfo
        {
                Id = video.Id,
                Title = video.Title,
                Author = video.Author.ChannelTitle,
                Duration = video.Duration?.TotalSeconds ?? 0,
                ProviderId = ServiceId,
                Thumbnail = GetThumbnails(video.Thumbnails).MediumRes,
                StreamUri = null
        };
    }

    private static (string LowRes, string MediumRes, string HighRes) GetThumbnails(IReadOnlyList<Thumbnail> thumbnails)
    {
        Thumbnail[] ordered = thumbnails.OrderBy(thumbRes => thumbRes.Resolution.Area).ToArray();
        return (thumbnails[0].Url, thumbnails[^1].Url,
                ordered.Length >= 2 ? ordered[ordered.Length / 2 - 1].Url : thumbnails[^1].Url);
    }
}