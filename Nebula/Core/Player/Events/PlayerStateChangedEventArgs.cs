using System;

namespace Nebula.Core.Player.Events
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