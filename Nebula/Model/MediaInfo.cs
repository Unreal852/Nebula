using System;
using System.Threading.Tasks;
using Nebula.Core;
using Nebula.Core.Extensions;
using Nebula.Core.Providers;
using SQLite;

namespace Nebula.Model
{
    [Table("Medias")]
    public class MediaInfo : IThumbnailSource
    {
        public MediaInfo()
        {
        }

        public MediaInfo(string id, string authorId, string title, string authorName, string providerName, string lowResThumbnail,
                         string mediumResThumbnail, string highResThumbnail, TimeSpan duration)
        {
            MediaId = id;
            AuthorId = authorId;
            Title = title;
            AuthorName = authorName;
            ProviderName = providerName;
            LowResThumbnail = lowResThumbnail;
            MediumResThumbnail = mediumResThumbnail;
            HighResThumbnail = highResThumbnail;
            Duration = duration;
            Title = Validator.ValidateMediaTitle(this);
        }

        [Indexed] [PrimaryKey] public string   MediaId                 { get; set; }
        public                        string   AuthorId                { get; set; }
        public                        string   AuthorName              { get; set; }
        public                        string   ProviderName            { get; set; }
        public                        string   Title                   { get; set; }
        public                        TimeSpan Duration                { get; set; }
        [Ignore] public               bool     IsActive                { get; set; } = true;
        public                        string   LowResThumbnail         { get; set; }
        public                        string   MediumResThumbnail      { get; set; }
        public                        string   HighResThumbnail        { get; set; }
        [Ignore] public               string   CustomThumbnail         { get; set; }
        public                        string   AnyThumbnailFromHighest => this.AnyThumbnailFromHighest();
        public                        string   AnyThumbnailFromLowest  => this.AnyThumbnailFromLowest();
        [Ignore] public               string   Url                     => $"https://www.youtube.com/watch?v={MediaId}";

        public async Task<ArtistInfo> GetArtistInfo()
        {
            return await GetMediaProvider().GetArtistInfo(AuthorId);
        }

        public async Task<Uri> GetAudioStreamUri()
        {
            return await GetMediaProvider().GetAudioStreamUri(this);
        }

        public async Task<Uri> GetVideoStreamUri()
        {
            return await GetMediaProvider().GetVideoStreamUri(this);
        }

        public async Task<Uri> GetMuxedStreamUri()
        {
            return await GetMediaProvider().GetMuxedStreamUri(this);
        }

        public IMediasProvider GetMediaProvider()
        {
            return NebulaClient.Providers.GetProvider(ProviderName);
        }

        public override string ToString()
        {
            return Title;
        }
    }
}