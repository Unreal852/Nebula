using Nebula.Common.Medias;
using System;

namespace Nebula.Desktop.Contracts;

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