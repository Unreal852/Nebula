using Nebula.Common.Medias;

namespace Nebula.Common.Audio.Events;

public class MediaChangingEventArgs : EventArgs
{
    public MediaChangingEventArgs(IMediaInfo? oldMedia, IMediaInfo newMedia)
    {
        OldMedia = oldMedia;
        NewMedia = newMedia;
    }

    public IMediaInfo? OldMedia { get; }
    public IMediaInfo  NewMedia { get; }
}