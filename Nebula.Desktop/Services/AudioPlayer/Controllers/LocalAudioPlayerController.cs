using System;
using Nebula.Common.Medias;
using Nebula.Services.Contracts;

// ReSharper disable InconsistentNaming

namespace Nebula.Desktop.Services.AudioPlayer.Controllers;

public sealed class LocalAudioPlayerController : IAudioPlayerController
{
    public bool OpenMedia(IMediaInfo media)
    {
        return true;
    }

    public bool OpenPlaylist(string playlistUrl)
    {
        return true;
    }

    public bool Play()
    {
        return true;
    }

    public bool Pause()
    {
        return true;
    }

    public bool Skip()
    {
        return true;
    }

    public bool Stop()
    {
        return true;
    }

    public bool SetPosition(in TimeSpan position)
    {
        return true;
    }
}