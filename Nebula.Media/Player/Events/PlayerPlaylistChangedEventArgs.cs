using System;

namespace Nebula.Media.Player.Events
{
    public class PlayerPlaylistChangedEventArgs : EventArgs
    {
        public PlayerPlaylistChangedEventArgs(IPlaylist old, IPlaylist @new)
        {
            OldPlaylist = old;
            NewPlaylist = @new;
        }

        /// <summary>
        /// The old <see cref="IPlaylist"/>, this can be null
        /// </summary>
        public IPlaylist OldPlaylist { get; }

        /// <summary>
        /// The new <see cref="IPlaylist"/>
        /// </summary>
        public IPlaylist NewPlaylist { get; }
    }
}