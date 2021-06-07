using System;

namespace Nebula.Media.Events
{
    public class PlaylistMediaRemovedEventArgs : EventArgs
    {
        public PlaylistMediaRemovedEventArgs(IPlaylist playlist, IMediaInfo mediaInfo)
        {
            Playlist = playlist ?? throw new ArgumentNullException(nameof(playlist));
            AddedMedia = mediaInfo ?? throw new ArgumentNullException(nameof(mediaInfo));
        }

        /// <summary>
        ///     Modified Playlist.
        /// </summary>
        public IPlaylist Playlist { get; }

        /// <summary>
        ///     Removed Media.
        /// </summary>
        public IMediaInfo AddedMedia { get; }
    }
}