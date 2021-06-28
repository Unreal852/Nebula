using System;
using System.Threading.Tasks;
using Nebula.Core;
using Nebula.Core.Extensions;
using Nebula.Core.Providers;
using Nebula.Core.Providers.Youtube;
using SQLite;

namespace Nebula.Model
{
    [Table("Medias")]
    public class MediaInfo : IThumbnailSource
    {
        public MediaInfo()
        {
        }

        public MediaInfo(string id, string authorId, string title, string author, string description, string providerName, string lowResThumbnail,
                         string mediumResThumbnail, string highResThumbnail, TimeSpan duration, DateTime creationDate)
        {
            Id = id;
            AuthorId = authorId;
            Title = title;
            Author = author;
            Description = description;
            ProviderName = providerName;
            LowResThumbnail = lowResThumbnail;
            MediumResThumbnail = mediumResThumbnail;
            HighResThumbnail = highResThumbnail;
            Duration = duration;
            CreationDate = creationDate;
            Title = Validator.ValidateMediaTitle(this);
        }

        [Indexed] [PrimaryKey] public string   Id                      { get; set; }
        public                        string   AuthorId                { get; set; }
        public                        string   Title                   { get; set; }
        public                        string   Author                  { get; set; }
        public                        string   Description             { get; set; }
        public                        string   ProviderName            { get; set; }
        public                        string   LowResThumbnail         { get; set; }
        public                        string   MediumResThumbnail      { get; set; }
        public                        string   HighResThumbnail        { get; set; }
        public                        string   CustomThumbnail         { get; set; }
        public                        TimeSpan Duration                { get; set; }
        public                        DateTime CreationDate            { get; set; }
        [Ignore] public               bool     IsActive                { get; set; } = true;
        public                        string   AnyThumbnailFromHighest => this.AnyThumbnailFromHighest();
        public                        string   AnyThumbnailFromLowest  => this.AnyThumbnailFromLowest();

        public Task<ArtistInfo> GetArtistInfo()
        {
            throw new NotImplementedException();
        }

        public async Task<Uri> GetAudioStreamUri()
        {
            return await NebulaClient.Providers.FindProviderByType<YoutubeMediaProvider>().GetAudioStreamUri(this);
        }

        public Task<Uri> GetVideoStreamUri()
        {
            throw new NotImplementedException();
        }

        public Task<Uri> GetMuxedStreamUri()
        {
            throw new NotImplementedException();
        }

        public IMediasProvider GetMediaProvider()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Title;
        }
    }
}