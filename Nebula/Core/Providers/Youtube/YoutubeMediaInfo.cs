using System;
using System.Linq;
using System.Threading.Tasks;
using Nebula.Media;
using Nebula.Model;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;

namespace Nebula.Core.Providers.Youtube
{
    public class YoutubeMediaInfo : MediaInfo
    {
        public YoutubeMediaInfo() : base()
        {
        }

        internal YoutubeMediaInfo(IVideo video)
        {
            Id = video.Id.Value;
            AuthorId = video.Author.ChannelId.Value;
            Title = video.Title;
            Author = video.Author.Title;
            Duration = video.Duration ?? TimeSpan.Zero;

            Thumbnail[] thumbnails = video.Thumbnails.OrderBy(thumbRes => thumbRes.Resolution.Area).ToArray();
            LowResThumbnailUrl = thumbnails[0].Url;
            HighResThumbnailUrl = thumbnails[^1].Url;
            MediumResThumbnailUrl = thumbnails.Length >= 2 ? thumbnails[thumbnails.Length / 2 - 1].Url : HighResThumbnailUrl;
        }

        public       IMediasProvider   GetMediaProvider()  => NebulaClient.Providers.FindProviderByType<YoutubeMediaProvider>();
        public async Task<IArtistInfo> GetArtistInfo()     => await GetMediaProvider().GetArtistInfo(AuthorId);
        public async Task<Uri>         GetAudioStreamUri() => await GetMediaProvider().GetAudioStreamUri(this);
        public async Task<Uri>         GetMuxedStreamUri() => await GetMediaProvider().GetMuxedStreamUri(this);
        public async Task<Uri>         GetVideoStreamUri() => await GetMediaProvider().GetVideoStreamUri(this);
    }
}