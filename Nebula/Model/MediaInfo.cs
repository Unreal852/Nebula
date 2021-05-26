using System;
using System.Threading.Tasks;
using Nebula.Core.Providers.Youtube;
using Nebula.Media;
using SQLite;

namespace Nebula.Model
{
    [Table("Medias")]
    public class MediaInfo : IMediaInfo
    {
        public MediaInfo()
        {
        }

        public MediaInfo(string id, string authorId, string title, string author, string description, string lowResThumbnailUrl, string mediumResThumbnailUrl,
                         string highResThumbnailUrl, TimeSpan duration, DateTime creationDate)
        {
            Id = id;
            AuthorId = authorId;
            Title = title;
            Author = author;
            Description = description;
            LowResThumbnailUrl = lowResThumbnailUrl;
            MediumResThumbnailUrl = mediumResThumbnailUrl;
            HighResThumbnailUrl = highResThumbnailUrl;
            Duration = duration;
            CreationDate = creationDate;
        }

        [Indexed, PrimaryKey] public string   Id                    { get; set; }
        public                       string   AuthorId              { get; set; }
        public                       string   Title                 { get; set; }
        public                       string   Author                { get; set; }
        public                       string   Description           { get; set; }
        public                       string   LowResThumbnailUrl    { get; set; }
        public                       string   MediumResThumbnailUrl { get; set; }
        public                       string   HighResThumbnailUrl   { get; set; }
        public                       TimeSpan Duration              { get; set; }
        public                       DateTime CreationDate          { get; set; }

        public IMediasProvider GetMediaProvider()
        {
            throw new NotImplementedException();
        }

        public Task<IArtistInfo> GetArtistInfo()
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

        public override string ToString() => Title;
    }
}