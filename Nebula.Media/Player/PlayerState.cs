namespace Nebula.Media.Player
{
    /// <summary>
    /// Represents the <see cref="IMediaPlayer"/> state
    /// </summary>
    public enum PlayerState
    {
        /// <summary>
        /// The player is idle/stopped
        /// </summary>
        Idle,

        /// <summary>
        /// The player is playing
        /// </summary>
        Playing,

        /// <summary>
        /// The player is paused
        /// </summary>
        Paused
    }
}