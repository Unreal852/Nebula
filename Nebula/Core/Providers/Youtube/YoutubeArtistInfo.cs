using System.Collections.Generic;
using Nebula.Media;

namespace Nebula.Core.Providers.Youtube
{
    public class YoutubeArtistInfo : IArtistInfo
    {
        public YoutubeArtistInfo(string id, string title, string url, string logoUrl)
        {
            Id = id;
            Name = title;
            Url = url;
            ThumbnailUri = logoUrl;
        }

        public string Id { get; }

        public string Name { get; }

        public string Url { get; }

        public string ThumbnailUri { get; }

        public IAsyncEnumerable<IMediaInfo> GetMedias()
        {
            return NebulaClient.Providers.FindProviderByType<YoutubeMediaProvider>().GetArtistMedias(Id);
        }
    }
}