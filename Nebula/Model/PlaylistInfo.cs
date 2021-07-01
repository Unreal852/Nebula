using System.Text.RegularExpressions;
using Nebula.Core;
using Nebula.Core.Extensions;
using Nebula.Core.Playlists;
using SQLite;

// ReSharper disable MemberCanBePrivate.Global

namespace Nebula.Model
{
    [Table("Playlists")]
    public class PlaylistInfo : IThumbnailSource
    {
        private const           int   MaxNameLength        = 128;
        private const           int   MaxDescriptionLength = 92;
        private const           int   MaxAuthorLength      = 128;
        private static readonly Regex SRemLines            = new(@"\t|\n|\r", RegexOptions.Compiled);

        public PlaylistInfo()
        {
            // For database deserialization
        }

        public PlaylistInfo(PlaylistMediasLoaderType loaderType, string playlistId, string authorId, string name, string authorName, string description,
                            string lowResThumbnail = "", string mediumResThumbnail = "", string highResThumbnail = "", string customThumbnail = "")
        {
            PlaylistId = playlistId;
            AuthorId = authorId;
            LoaderType = loaderType;
            Name = name;
            Description = description;
            AuthorName = authorName;
            LowResThumbnail = lowResThumbnail;
            MediumResThumbnail = mediumResThumbnail;
            HighResThumbnail = highResThumbnail;
            CustomThumbnail = customThumbnail;
            ValidateFields();
        }

        [PrimaryKey] [AutoIncrement] public int                      PlaylistIndex      { get; set; }
        public                              string                   PlaylistId         { get; set; }
        public                              string                   AuthorId           { get; set; }
        public                              string                   Name               { get; set; }
        public                              string                   Description        { get; set; }
        public                              string                   AuthorName         { get; set; }
        public                              string                   LowResThumbnail    { get; set; }
        public                              string                   MediumResThumbnail { get; set; }
        public                              string                   HighResThumbnail   { get; set; }
        public                              string                   CustomThumbnail    { get; set; }
        public                              PlaylistMediasLoaderType LoaderType         { get; set; }

        public string AnyThumbnailFromHighest => this.AnyThumbnailFromHighest();
        public string AnyThumbnailFromLowest  => this.AnyThumbnailFromLowest();

        public void ValidateFields()
        {
            Name = string.IsNullOrWhiteSpace(Name) ? "Unnamed Playlist" : SRemLines.Replace(Name, "").Truncate(MaxNameLength);
            Description = string.IsNullOrWhiteSpace(Description) ? " " : SRemLines.Replace(Description, "").Truncate(MaxDescriptionLength);
            AuthorName = string.IsNullOrWhiteSpace(AuthorName) ? "Unnamed Author" : SRemLines.Replace(AuthorName, "").Truncate(MaxAuthorLength);
        }
    }
}