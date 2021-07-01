using System;
using Nebula.Model;

namespace Nebula.Core.Playlists.Events
{
    public class PlaylistMediaAddedEventArgs : EventArgs
    {
        public PlaylistMediaAddedEventArgs(Playlist playlist, MediaInfo mediaInfo, int mediaIndex)
        {
            Playlist = playlist ?? throw new ArgumentNullException(nameof(playlist));
            AddedMedia = mediaInfo ?? throw new ArgumentNullException(nameof(mediaInfo));
            MediaIndex = mediaIndex;
        }

        /// <summary>
        ///     Modified Playlist.
        /// </summary>
        public Playlist Playlist { get; }

        /// <summary>
        ///     New Media.
        /// </summary>
        public MediaInfo AddedMedia { get; }

        /// <summary>
        ///     New Media Insert PlaylistIndex.
        /// </summary>
        public int MediaIndex { get; }
    }
}