using System;
using System.Collections;
using System.Collections.Generic;
using Nebula.Media;
using Nebula.Media.Events;
using SQLite;

namespace Nebula.Model
{
    [Table("Playlists")]
    public class Playlist : IPlaylist
    {
        public Playlist()
        {
        }

        public Playlist(string name, string description, string author, Uri thumbnail, ICollection<IMediaInfo> medias) : this()
        {
            Name = name;
            Description = description;
            Author = author;
            Thumbnail = thumbnail;
            Medias.SetElements(medias);
        }

        [PrimaryKey, AutoIncrement] public int              Id            { get; set; }
        public                             string           Name          { get; set; }
        public                             string           Description   { get; set; }
        public                             string           Author        { get; set; }
        public                             Uri              Thumbnail     { get; set; }
        [Ignore] public                    MediasCollection Medias        { get; } = new();
        public                             int              MediasCount   => Medias.Count;
        public                             TimeSpan         TotalDuration => Medias.TotalDuration;

        public event EventHandler<PlaylistMediaAddedEventArgs>   MediaAdded;
        public event EventHandler<PlaylistMediaRemovedEventArgs> MediaRemoved;

        public IMediaInfo this[int index] => Medias[index];
        public IMediaInfo              GetMedia(int index)                      => Medias[index];
        public IEnumerator<IMediaInfo> GetEnumerator()                          => Medias.GetEnumerator();
        public bool                    Contains(IMediaInfo mediaInfo)           => Medias.Elements.Contains(mediaInfo);
        public void                    AddMedias(params IMediaInfo[] medias)    => Medias.AddRange(medias);
        public void                    RemoveMedias(params IMediaInfo[] medias) => Medias.RemoveRange(medias);
        IEnumerator IEnumerable.       GetEnumerator()                          => GetEnumerator();

        public void RemoveMedia(IMediaInfo mediaInfo) => Medias.Remove(mediaInfo);

        public void AddMedia(IMediaInfo mediaInfo, int insertIndex = -1)
        {
            if (insertIndex < 0)
                Medias.Add(mediaInfo);
            else
                Medias.Insert(mediaInfo, insertIndex);
            NebulaClient.Database.InsertPlaylistMedia(this, mediaInfo, insertIndex < 0 ? MediasCount - 1 : insertIndex);
        }
    }
}