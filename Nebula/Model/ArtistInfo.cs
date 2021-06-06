using System.Collections.Generic;
using Nebula.Media;
using Nebula.Media.Extensions;
using SQLite;

namespace Nebula.Model
{
    [Table("Artists")]
    public class ArtistInfo : IArtistInfo
    {
        public ArtistInfo()
        {
        }

        public ArtistInfo(string id, string name, string url, string lowResThumbnail, string mediumResThumbnail, string highResThumbnail)
        {
            Id = id;
            Name = name;
            Url = url;
            LowResThumbnail = lowResThumbnail;
            MediumResThumbnail = mediumResThumbnail;
            HighResThumbnail = highResThumbnail;
        }

        [Indexed, PrimaryKey] public string Id                      { get; set; }
        public                       string Name                    { get; set; }
        public                       string Url                     { get; set; }
        public                       string LowResThumbnail         { get; set; }
        public                       string MediumResThumbnail      { get; set; }
        public                       string HighResThumbnail        { get; set; }
        public                       string CustomThumbnail         { get; set; }
        public                       string AnyThumbnailFromHighest => this.AnyThumbnailFromHighest();
        public                       string AnyThumbnailFromLowest  => this.AnyThumbnailFromLowest();

        public IAsyncEnumerable<IMediaInfo> GetMedias()
        {
            throw new System.NotImplementedException();
        }
    }
}