using System;

namespace Nebula.Core.Player.Events
{
    public class PlayerMuteChangedEventArgs : EventArgs
    {
        public PlayerMuteChangedEventArgs(bool muted)
        {
            IsMuted = muted;
        }

        /// <summary>
        ///     True if the sound is muted, false otherwise.
        /// </summary>
        public bool IsMuted { get; }
    }
}