using System;

namespace Nebula.Media.Player.Events
{
    public class PlayerShuffleChangedEventArgs : EventArgs
    {
        public PlayerShuffleChangedEventArgs(bool shuffle)
        {
            Shuffle = shuffle;
        }

        /// <summary>
        /// True if shuffle is enabled, false otherwise.
        /// </summary>
        public bool Shuffle { get; }
    }
}