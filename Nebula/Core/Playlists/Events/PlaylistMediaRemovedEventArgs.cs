using System;
using Nebula.Model;

namespace Nebula.Core.Playlists.Events
{
    public class PlaylistMediaRemovedEventArgs : EventArgs
    {
        public PlaylistMediaRemovedEventArgs(Playlist playlist, MediaInfo mediaInfo)
        {
            Playlist = playlist ?? throw new ArgumentNullException(nameof(playlist));
            AddedMedia = mediaInfo ?? throw new ArgumentNullException(nameof(mediaInfo));
        }

        /// <summary>
        ///     Modified Playlist.
        /// </summary>
        public Playlist Playlist { get; }

        /// <summary>
        ///     Removed Media.
        /// </summary>
        public MediaInfo AddedMedia { get; }
    }
}