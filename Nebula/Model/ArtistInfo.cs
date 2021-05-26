using System.Collections.Generic;
using Nebula.Media;
using SQLite;

namespace Nebula.Model
{
    [Table("Artists")]
    public class ArtistInfo : IArtistInfo
    {
        public ArtistInfo()
        {
        }

        public ArtistInfo(string id, string name, string url, string thumbnailUri)
        {
            Id = id;
            Name = name;
            Url = url;
            ThumbnailUri = thumbnailUri;
        }

        [Indexed, PrimaryKey] public string Id           { get; set; }
        public                       string Name         { get; set; }
        public                       string Url          { get; set; }
        public                       string ThumbnailUri { get; set; }

        public IAsyncEnumerable<IMediaInfo> GetMedias()
        {
            throw new System.NotImplementedException();
        }
    }
}