using Nebula.Common.Audio;
using Nebula.Common.Audio.Events;
using Nebula.Common.Medias;
using Nebula.Common.Playlist;

namespace Nebula.Services.Contracts;

public interface IAudioPlayerService
{
    IMediaInfo? CurrentMedia { get; }
    bool        IsPlaying    { get; }
    bool        IsPaused     { get; }
    bool        IsStopped    { get; }
    bool        Shuffle      { get; set; }
    float       Volume       { get; set; }
    int         QueuedMedias { get; }

    event EventHandler<MediaChangingEventArgs>? MediaChanging;

    Task                    OpenMedia(IMediaInfo media, bool force = false);
    Task                    OpenPlaylist(Playlist playlist, bool force = false);
    Task                    Skip(bool force = false);
    void                    Play(bool force = false);
    void                    Pause(bool force = false);
    void                    Stop(bool force = false);
    void                    SetPosition(in TimeSpan position, bool force = false);
    IAudioPlayerController? UpdateController(AudioPlayerControllerType audioPlayerControllerType);
}