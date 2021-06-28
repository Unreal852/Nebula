using System.Collections.Generic;
using Nebula.Core;
using Nebula.Core.Extensions;
using SQLite;

namespace Nebula.Model
{
    [Table("Artists")]
    public class ArtistInfo : IThumbnailSource
    {
        public ArtistInfo()
        {
        }

        public ArtistInfo(string authorId, string providerName, string name, string lowResThumbnail, string mediumResThumbnail, string highResThumbnail)
        {
            AuthorId = authorId;
            ProviderName = providerName;
            Name = name;
            LowResThumbnail = lowResThumbnail;
            MediumResThumbnail = mediumResThumbnail;
            HighResThumbnail = highResThumbnail;
        }

        [Indexed, PrimaryKey] public string AuthorId                { get; set; }
        public                       string ProviderName            { get; set; }
        public                       string Name                    { get; set; }
        public                       string LowResThumbnail         { get; set; }
        public                       string MediumResThumbnail      { get; set; }
        public                       string HighResThumbnail        { get; set; }
        [Ignore] public              string CustomThumbnail         { get; set; }
        public                       string Url                     => $"https://www.youtube.com/channel/{AuthorId}";
        public                       string AnyThumbnailFromHighest => this.AnyThumbnailFromHighest();
        public                       string AnyThumbnailFromLowest  => this.AnyThumbnailFromLowest();

        public async IAsyncEnumerable<MediaInfo> GetMedias()
        {
            await foreach (MediaInfo mediaInfo in NebulaClient.Providers.GetProvider(ProviderName).GetArtistMedias(this))
                yield return mediaInfo;
        }
    }
}