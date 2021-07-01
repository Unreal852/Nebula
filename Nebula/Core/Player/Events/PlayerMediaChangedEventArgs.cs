using System;
using Nebula.Core.Playlists;
using Nebula.Model;

namespace Nebula.Core.Player.Events
{
    public class PlayerMediaChangedEventArgs : EventArgs
    {
        public PlayerMediaChangedEventArgs(Playlist playlist, MediaInfo oldMedia, MediaInfo newMedia)
        {
            Playlist = playlist;
            OldMedia = oldMedia;
            NewMedia = newMedia;
        }

        /// <summary>
        ///     Currently Played <see cref="IPlaylist" />. This can be null if no playlist is being played.
        /// </summary>
        public Playlist Playlist { get; }

        /// <summary>
        ///     Previous media <see cref="IMediaInfo" />. This can be null if there was no previous media.
        /// </summary>
        public MediaInfo OldMedia { get; }

        /// <summary>
        ///     The new <see cref="IMediaInfo" /> being played. This can't be null.
        /// </summary>
        public MediaInfo NewMedia { get; }
    }
}