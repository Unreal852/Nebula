using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nebula.Core;
using Nebula.Core.Database;
using Nebula.Media;
using Nebula.Media.Events;
using Nebula.Media.Extensions;
using Nebula.Utils.Extensions;
using SQLite;

namespace Nebula.Model
{
    [Table("Playlists")]
    public class Playlist : IPlaylist, IEnumerable<MediaInfo>
    {
        private const           int             MaxNameLength        = 128;
        private const           int             MaxDescriptionLength = 128;
        private const           int             MaxAuthorLength      = 128;
        private static readonly Regex           SRemLines            = new(@"\t|\n|\r");
        private static readonly IPlaylistLoader DefaultLoader        = new DatabasePlaylistLoader();

        private ObservableCollection<MediaInfo> _medias;

        public Playlist()
        {
            Medias = new();
            Medias.CollectionChanged += OnCollectionChanged;
        }

        public Playlist(string name, string description, string author, string lowResThumbnail, string mediumResThumbnail, string highResThumbnail,
                        ICollection<MediaInfo> medias) : this()
        {
            Name = name;
            Description = description;
            Author = author;
            LowResThumbnail = lowResThumbnail;
            MediumResThumbnail = mediumResThumbnail;
            HighResThumbnail = highResThumbnail;
            ValidateFields();
            if (medias is {Count: > 0})
            {
                foreach (MediaInfo media in medias)
                    Medias.Add(media);
                IsLoaded = true;
            }
        }

        [PrimaryKey, AutoIncrement] public int                               Id                      { get; set; }
        public                             string                            Name                    { get; set; }
        public                             string                            Description             { get; set; }
        public                             string                            Url                     { get; set; }
        public                             string                            Author                  { get; set; }
        public                             string                            LowResThumbnail         { get; set; }
        public                             string                            MediumResThumbnail      { get; set; }
        public                             string                            HighResThumbnail        { get; set; }
        public                             string                            CustomThumbnail         { get; set; }
        public                             bool                              KeepSync                { get; set; }
        public                             string                            ProviderName            { get; set; }
        [Ignore] public                    bool                              AutoSave                { get; set; }           = true;
        [Ignore] public                    bool                              IsLoaded                { get; protected set; } = false;
        [Ignore] public                    ObservableCollectionEx<MediaInfo> Medias                  { get; }
        [Ignore] private                   IPlaylistLoader                   PlaylistLoader          { get; set; }
        public                             TimeSpan                          TotalDuration           { get; private set; }
        public                             int                               MediasCount             => Medias.Count;
        public                             string                            AnyThumbnailFromHighest => this.AnyThumbnailFromHighest();
        public                             string                            AnyThumbnailFromLowest  => this.AnyThumbnailFromLowest();

        public event EventHandler<PlaylistMediaAddedEventArgs>   MediaAdded;
        public event EventHandler<PlaylistMediaRemovedEventArgs> MediaRemoved;

        public MediaInfo this[int index] => Medias[index];
        public IMediaInfo             GetMedia(int index)           => Medias[index];
        public bool                   Contains(MediaInfo mediaInfo) => Medias.Contains(mediaInfo);
        public IEnumerator<MediaInfo> GetEnumerator()               => Medias.GetEnumerator();
        IEnumerator IEnumerable.      GetEnumerator()               => GetEnumerator();

        public void ValidateFields()
        {
            Name = string.IsNullOrWhiteSpace(Name) ? " " : SRemLines.Replace(Name, "").Truncate(MaxNameLength);
            Description = string.IsNullOrWhiteSpace(Description) ? " " : SRemLines.Replace(Description, "").Truncate(MaxDescriptionLength);
            Author = string.IsNullOrWhiteSpace(Author) ? " " : SRemLines.Replace(Author, "").Truncate(MaxAuthorLength);
        }

        public virtual async Task Load()
        {
            if (IsLoaded)
                return;
            IsLoaded = await (PlaylistLoader?.LoadPlaylist(this) ?? DefaultLoader.LoadPlaylist(this));
        }

        public IEnumerable<MediaInfo> GetActiveMedias()
        {
            foreach (MediaInfo mediaInfo in Medias)
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
                Medias.Insert(insertIndex, mediaInfo);
            if (AutoSave)
                NebulaClient.Database.InsertPlaylistMedia(this, mediaInfo, insertIndex < 0 ? MediasCount - 1 : insertIndex);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    if (e.NewItems != null)
                    {
                        foreach (MediaInfo newMedia in e.NewItems)
                            TotalDuration += newMedia.Duration;
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    if (e.OldItems != null)
                    {
                        foreach (MediaInfo newMedia in e.OldItems)
                            TotalDuration -= newMedia.Duration;
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Replace:
                {
                    if (e.OldItems != null)
                    {
                        foreach (MediaInfo newMedia in e.OldItems)
                            TotalDuration -= newMedia.Duration;
                    }

                    if (e.NewItems != null)
                    {
                        foreach (MediaInfo newMedia in e.NewItems)
                            TotalDuration += newMedia.Duration;
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Reset:
                {
                    TotalDuration = TimeSpan.Zero;
                    foreach (MediaInfo mediaInfo in Medias)
                        TotalDuration += mediaInfo.Duration;
                    break;
                }
            }
        }
    }
}