namespace Nebula.Common.Audio.Events;

public class AudioServiceStateChangedEventArgs : EventArgs
{
    public AudioServiceStateChangedEventArgs(AudioServiceState state)
    {
        State = state;
    }

    public AudioServiceState State { get; }
}