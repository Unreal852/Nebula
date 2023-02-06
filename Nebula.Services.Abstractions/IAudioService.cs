using Nebula.Common.Audio;
using Nebula.Common.Audio.Events;
using Nebula.Common.Medias;

namespace Nebula.Services.Abstractions;

public interface IAudioService
{
    IMediaInfo?       CurrentMedia { get; }
    AudioServiceState State        { get; }
    TimeSpan          Duration     { get; }
    TimeSpan          Position     { get; set; }
    bool              IsPlaying    { get; }
    bool              IsPaused     { get; }
    bool              IsStopped    { get; }
    float             Volume       { get; set; }
    IPositionChangedHandler? PositionChangedHandler { get; set; }  // TODO: This is bad

    event EventHandler<MediaChangedEventArgs>?             MediaChanged;
    event EventHandler<AudioServiceStateChangedEventArgs>? StateChanged;
    event EventHandler<EventArgs>?                         PlaybackStopped;

    void OpenMedia(IMediaInfo media);
    void Play();
    void Pause();
    void Stop();
    void Shutdown();
}