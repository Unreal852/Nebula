using Nebula.Common.Medias;

namespace Nebula.Common.Audio.Events;

public class MediaChangedEventArgs : EventArgs
{
    public MediaChangedEventArgs(IMediaInfo? oldMedia, IMediaInfo newMedia)
    {
        OldMedia = oldMedia;
        NewMedia = newMedia;
    }

    public IMediaInfo? OldMedia { get; }
    public IMediaInfo  NewMedia { get; }
}