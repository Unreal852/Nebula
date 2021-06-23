using System;

namespace Nebula.Core.Player.Events
{
    public class PlayerVolumeChangedEventArgs : EventArgs
    {
        public PlayerVolumeChangedEventArgs(int oldVolume, int newVolume)
        {
            OldVolume = oldVolume;
            NewVolume = newVolume;
        }

        /// <summary>
        ///     Previous volume value.
        /// </summary>
        public int OldVolume { get; }

        /// <summary>
        ///     New volume value.
        /// </summary>
        public int NewVolume { get; }
    }
}