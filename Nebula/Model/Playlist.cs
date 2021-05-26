using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nebula.Core;
using Nebula.Media;
using Nebula.Media.Events;
using Nebula.Utils.Extensions;
using SQLite;

namespace Nebula.Model
{
    [Table("Playlists")]
    public class Playlist : IPlaylist, IEnumerable<MediaInfo>
    {
        private const           int   MaxNameLength        = 128;
        private const           int   MaxDescriptionLength = 128;
        private const           int   MaxAuthorLength      = 128;
        private static readonly Regex SRemLines            = new(@"\t|\n|\r");

        public Playlist()
        {
            Medias.MaxElementsPerPage = 25;
        }

        public Playlist(string name, string description, string author, Uri thumbnail, ICollection<MediaInfo> medias) : this()
        {
            Name = string.IsNullOrWhiteSpace(name) ? " " : SRemLines.Replace(name, "").Truncate(MaxNameLength);
            Description = string.IsNullOrWhiteSpace(description) ? " " : SRemLines.Replace(description, "").Truncate(MaxDescriptionLength);
            Author = string.IsNullOrWhiteSpace(author) ? " " : SRemLines.Replace(author, "").Truncate(MaxAuthorLength);
            Thumbnail = thumbnail ?? new Uri("https://i.imgur.com/Od5XogD.png");
            Medias.SetElements(medias);
        }

        [PrimaryKey, AutoIncrement] public int                         Id            { get; set; }
        public                             string                      Name          { get; set; }
        public                             string                      Description   { get; set; }
        public                             string                      Author        { get; set; }
        public                             Uri                         Thumbnail     { get; set; }
        [Ignore] public                    bool                        AutoSave      { get; set; } = true;
        [Ignore] public                    MediasCollection<MediaInfo> Medias        { get; }      = new();
        public                             int                         MediasCount   => Medias.Count;
        public                             TimeSpan                    TotalDuration => Medias.TotalDuration;

        public event EventHandler<PlaylistMediaAddedEventArgs>   MediaAdded;
        public event EventHandler<PlaylistMediaRemovedEventArgs> MediaRemoved;

        public MediaInfo this[int index] => Medias.Elements[index];
        public IMediaInfo             GetMedia(int index)                     => Medias[index];
        public bool                   Contains(MediaInfo mediaInfo)           => Medias.Elements.Contains(mediaInfo);
        public void                   AddMedias(params MediaInfo[] medias)    => Medias.AddRange(medias);
        public void                   RemoveMedias(params MediaInfo[] medias) => Medias.RemoveRange(medias);
        public IEnumerator<MediaInfo> GetEnumerator()                         => Medias.GetEnumerator();
        IEnumerator IEnumerable.      GetEnumerator()                         => GetEnumerator();

        public IEnumerator<MediaInfo> GetActiveMedias()
        {
            foreach (MediaInfo mediaInfo in Medias.Elements)
            {
                if (mediaInfo.IsActive)
                    yield return mediaInfo;
            }
        }

        public void AddMedia(MediaInfo mediaInfo, int insertIndex = -1)
        {
            if (insertIndex < 0)
                Medias.Add(mediaInfo);
            else
                Medias.Insert(mediaInfo, insertIndex);
            if (AutoSave)
                NebulaClient.Database.InsertPlaylistMedia(this, mediaInfo, insertIndex < 0 ? MediasCount - 1 : insertIndex);
        }
    }
}