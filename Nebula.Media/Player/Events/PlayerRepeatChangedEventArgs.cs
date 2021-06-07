using System;

namespace Nebula.Media.Player.Events
{
    public class PlayerRepeatChangedEventArgs : EventArgs
    {
        public PlayerRepeatChangedEventArgs(bool repeat)
        {
            Repeat = repeat;
        }

        /// <summary>
        ///     True if repeat is enabled, false otherwise
        /// </summary>
        public bool Repeat { get; }
    }
}