using System;

namespace Nebula.Media.Player.Events
{
    public class PlayerStateChangedEventArgs : EventArgs
    {
        public PlayerStateChangedEventArgs(PlayerState newState)
        {
            NewState = newState;
        }

        public PlayerState NewState { get; }
    }
}