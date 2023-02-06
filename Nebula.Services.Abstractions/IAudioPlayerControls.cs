using Nebula.Common.Medias;

namespace Nebula.Services.Abstractions;

public interface IAudioPlayerControls
{
    void OpenMedia(MediaInfo media);
    void OpenPlaylist(string playlistUrl); // TODO: string is only to fast test this, we need a proper object type
    void Play();
    void Pause();
    void Skip();
    void Stop();
    void SetPosition(TimeSpan position);
}