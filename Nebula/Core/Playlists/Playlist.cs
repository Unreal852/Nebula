using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Nebula.Core.Playlists.Events;
using Nebula.Model;
using SQLite;

namespace Nebula.Core.Playlists
{
    [Table("Playlists")]
    public class Playlist : IEnumerable<MediaInfo>
    {
        public Playlist(PlaylistInfo playlistInfo, ICollection<MediaInfo> medias = null)
        {
            Info = playlistInfo;
            Medias = new ObservableCollectionEx<MediaInfo>();
            Medias.CollectionChanged += OnCollectionChanged;
            if (medias is {Count: > 0})
            {
                Medias.AddRange(medias);
                IsLoaded = true;
            }
        }

        public PlaylistInfo                      Info          { get; set; }
        public TimeSpan                          TotalDuration { get; private set; }
        public bool                              AutoSave      { get; set; } = true;
        public bool                              IsLoaded      { get; set; }
        public ObservableCollectionEx<MediaInfo> Medias        { get; }
        public int                               MediasCount   => Medias.Count;

        public MediaInfo this[int index] => Medias[index];

        public IEnumerator<MediaInfo> GetEnumerator()
        {
            return Medias.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public MediaInfo GetMedia(int index)
        {
            return Medias[index];
        }

        public bool Contains(MediaInfo mediaInfo)
        {
            return Medias.Contains(mediaInfo);
        }

        public event EventHandler<PlaylistMediaAddedEventArgs>   MediaAdded;
        public event EventHandler<PlaylistMediaRemovedEventArgs> MediaRemoved;

        public virtual async Task Load()
        {
            if (IsLoaded)
                return;
            IsLoaded = await NebulaClient.Playlists.GetLoader(Info.LoaderType).LoadPlaylist(this);
        }

        public IEnumerable<MediaInfo> GetActiveMedias()
        {
            foreach (MediaInfo mediaInfo in Medias)
                if (mediaInfo.IsActive)
                    yield return mediaInfo;
        }

        public async void AddMedia(MediaInfo mediaInfo, int insertIndex = -1)
        {
            if (insertIndex < 0)
                Medias.Add(mediaInfo);
            else
                Medias.Insert(insertIndex, mediaInfo);
            if (AutoSave)
                await NebulaClient.Database.InsertPlaylistMedia(this, mediaInfo, insertIndex < 0 ? MediasCount - 1 : insertIndex);
            MediaAdded?.Invoke(this, new PlaylistMediaAddedEventArgs(this, mediaInfo, insertIndex < 0 ? MediasCount - 1 : insertIndex));
        }

        public async void RemoveMedia(MediaInfo mediaInfo)
        {
            if (Medias.Contains(mediaInfo))
                Medias.Remove(mediaInfo);
            if (AutoSave)
                await NebulaClient.Database.RemovePlaylistMedia(this, mediaInfo);
            MediaRemoved?.Invoke(this, new PlaylistMediaRemovedEventArgs(this, mediaInfo));
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    if (e.NewItems != null)
                        foreach (MediaInfo newMedia in e.NewItems)
                            TotalDuration += newMedia.Duration;

                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    if (e.OldItems != null)
                        foreach (MediaInfo newMedia in e.OldItems)
                            TotalDuration -= newMedia.Duration;

                    break;
                }
                case NotifyCollectionChangedAction.Replace:
                {
                    if (e.OldItems != null)
                        foreach (MediaInfo newMedia in e.OldItems)
                            TotalDuration -= newMedia.Duration;

                    if (e.NewItems != null)
                        foreach (MediaInfo newMedia in e.NewItems)
                            TotalDuration += newMedia.Duration;

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