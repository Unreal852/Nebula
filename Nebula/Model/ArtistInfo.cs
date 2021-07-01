using System.Collections.Generic;
using Nebula.Core;
using Nebula.Core.Extensions;
using Nebula.Core.Providers;
using SQLite;

namespace Nebula.Model
{
    [Table("Artists")]
    public class ArtistInfo : IThumbnailSource
    {
        public ArtistInfo()
        {
            // For database deserialization
        }

        public ArtistInfo(ProviderType providerType, string artistId, string name, string lowResThumbnail, string mediumResThumbnail, string highResThumbnail)
        {
            ArtistId = artistId;
            ProviderType = providerType;
            Name = name;
            LowResThumbnail = lowResThumbnail;
            MediumResThumbnail = mediumResThumbnail;
            HighResThumbnail = highResThumbnail;
        }

        [Indexed, PrimaryKey] public string       ArtistId                { get; set; }
        public                       ProviderType ProviderType            { get; set; }
        public                       string       Name                    { get; set; }
        public                       string       LowResThumbnail         { get; set; }
        public                       string       MediumResThumbnail      { get; set; }
        public                       string       HighResThumbnail        { get; set; }
        public                       string       CustomThumbnail         { get; set; }
        public                       string       AnyThumbnailFromHighest => this.AnyThumbnailFromHighest();
        public                       string       AnyThumbnailFromLowest  => this.AnyThumbnailFromLowest();

        public async IAsyncEnumerable<MediaInfo> GetMedias()
        {
            await foreach (MediaInfo mediaInfo in NebulaClient.Providers.GetProvider(ProviderType).GetArtistMedias(ArtistId))
                yield return mediaInfo;
        }
    }
}