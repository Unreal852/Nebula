using System;
using Nebula.Model;

namespace Nebula.Core.Player.Events
{
    public class PlayerPlaylistChangedEventArgs : EventArgs
    {
        public PlayerPlaylistChangedEventArgs(Playlist old, Playlist @new)
        {
            OldPlaylist = old;
            NewPlaylist = @new;
        }

        /// <summary>
        ///     The old <see cref="IPlaylist" />, this can be null
        /// </summary>
        public Playlist OldPlaylist { get; }

        /// <summary>
        ///     The new <see cref="IPlaylist" />
        /// </summary>
        public Playlist NewPlaylist { get; }
    }
}