using System.Collections.Generic;
using System.Linq;
using YoutubeExplode.Common;

namespace Nebula.Core.Providers.Youtube
{
    public static class ThumbnailsExtensions
    {
        public static (string LowRes, string MediumRes, string HighRes) GetThumbnails(this IReadOnlyList<Thumbnail> thumbnails)
        {
            Thumbnail[] ordered = thumbnails.OrderBy(thumbRes => thumbRes.Resolution.Area).ToArray();
            return (thumbnails[0].Url, thumbnails[^1].Url, ordered.Length >= 2 ? ordered[ordered.Length / 2 - 1].Url : thumbnails[^1].Url);
        }
    }
}