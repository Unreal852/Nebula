using Nebula.Common.Medias;

namespace Nebula.Services.Contracts;

/// <summary>
/// Manage the <see cref="IAudioPlayerService"/> behaviour
/// </summary>
public interface IAudioPlayerController
{
    bool OpenMedia(IMediaInfo media);
    bool OpenPlaylist(string playlistUrl); // TODO: string is only to fast test this, we need a proper object type
    bool Play();
    bool Pause();
    bool Skip();
    bool Stop();
    bool SetPosition(in TimeSpan position);

    void Initialize(params object[] args)
    {
    }

    void OnRemoved()
    {
    }
}