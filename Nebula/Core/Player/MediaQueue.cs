using System;
using System.Collections;
using System.Collections.Generic;
using Nebula.Model;

namespace Nebula.Core.Player
{
    public class MediaQueue : IEnumerable<MediaInfo>
    {
        private static readonly Random Random = new();

        public MediaQueue()
        {
            Clear();
        }

        public  ObservableCollectionEx<MediaInfo> Queue         { get; } = new();
        private ObservableCollectionEx<MediaInfo> RecentDequeue { get; } = new();
        public  bool                              IsEmpty       => Queue.Count == 0;
        public  int                               Count         => Queue.Count;

        public IEnumerator<MediaInfo> GetEnumerator() => Queue.GetEnumerator();
        IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();


        /// <summary>
        /// Enqueue the specified <see cref="IPlaylist"/>
        /// </summary>
        /// <param name="playlist">The playlist to enqueue</param>
        /// <param name="clear">Clear current queue</param>
        public void Enqueue(Playlist playlist, bool clear = true)
        {
            if (clear)
                Clear();
            Queue.AddRange(playlist.GetActiveMedias());
        }

        /// <summary>
        /// Enqueue the specified <see cref="IMediaInfo"/>
        /// </summary>
        /// <param name="mediaInfo">The media to enqueue</param>
        /// <param name="insertIndex">The insert index</param>
        public void Enqueue(MediaInfo mediaInfo, int insertIndex = -1)
        {
            if (insertIndex == -1)
                Queue.Add(mediaInfo);
            else
                Queue.Insert(insertIndex, mediaInfo);
        }

        /// <summary>
        /// Remove the specified <see cref="IMediaInfo"/> from the queue
        /// </summary>
        /// <param name="mediaInfo">The media to remove</param>
        public void Remove(MediaInfo mediaInfo)
        {
            if (Queue.Contains(mediaInfo))
                Queue.Remove(mediaInfo);
        }

        /// <summary>
        /// Clear queue
        /// </summary>
        public void Clear()
        {
            Queue.Clear();
            RecentDequeue.Clear();
        }

        /// <summary>
        /// Check if the specified <see cref="IMediaInfo"/> is already queued.
        /// </summary>
        /// <param name="mediaInfo">The media to check</param>
        /// <returns>Returns true if the specified <see cref="IMediaInfo"/> is already queued, false otherwise.</returns>
        public bool IsQueued(MediaInfo mediaInfo)
        {
            return Queue.Contains(mediaInfo);
        }

        /// <summary>
        /// Dequeue a <see cref="IMediaInfo"/>.
        /// </summary>
        /// <param name="random">Queue randomly</param>
        /// <returns>Dequeued media info if queue is not empty, null otherwise</returns>
        public MediaInfo Dequeue(bool random = false)
        {
            if (IsEmpty)
                return default;
            MediaInfo mediaInfo = Queue[random ? Random.Next(Queue.Count) : 0];
            Queue.Remove(mediaInfo);
            RecentDequeue.Add(mediaInfo);
            return mediaInfo;
        }

        /// <summary>
        /// Rewind a dequeue.
        /// </summary>
        /// <returns>Rewinded Media</returns>
        public MediaInfo RewindDequeue()
        {
            if (RecentDequeue.Count == 0)
                return null;
            MediaInfo mediaInfo = RecentDequeue[^1];
            RecentDequeue.Remove(mediaInfo);
            //Enqueue(dequeueInfo.MediaInfo, dequeueInfo.Index);
            return mediaInfo;
        }
    }
}