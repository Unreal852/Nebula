using System;

namespace Nebula.Core.Player.Events
{
    public class PlayerPositionChangedEventArgs : EventArgs
    {
        public PlayerPositionChangedEventArgs(TimeSpan position, TimeSpan duration)
        {
            Position = position;
            Duration = duration;
        }

        /// <summary>
        ///     The current player position
        /// </summary>
        public TimeSpan Position { get; }

        /// <summary>
        ///     The current player total duration
        /// </summary>
        public TimeSpan Duration { get; }

        /// <summary>
        ///     The remaining time
        /// </summary>
        public TimeSpan Remaining => Duration - Position;
    }
}