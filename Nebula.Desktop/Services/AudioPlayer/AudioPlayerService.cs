using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using Nebula.Common.Audio;
using Nebula.Common.Audio.Events;
using Nebula.Common.Medias;
using Nebula.Common.Playlist;
using Nebula.Desktop.Services.AudioPlayer.Controllers;
using Nebula.Services.Contracts;

namespace Nebula.Desktop.Services.AudioPlayer;

public class AudioPlayerService : IAudioPlayerService
{
    private readonly IAudioService _audioService;
    private readonly List<IMediaInfo> _mediasQueue = new();
    private readonly IMediasProviderService _youtubeService;

    public AudioPlayerService(IAudioService audioService, IMediasProviderService youtubeService)
    {
        _audioService = audioService;
        _youtubeService = youtubeService;
        _audioService.PlaybackStopped += OnPlaybackStopped;
        AudioPlayerController = UpdateController(AudioPlayerControllerType.Default);
    }

    private IAudioPlayerController AudioPlayerController { get; set; }

    public bool Shuffle { get; set; }
    public IMediaInfo? CurrentMedia => _audioService.CurrentMedia;
    public bool IsPlaying => _audioService.IsPlaying;
    public bool IsPaused => _audioService.IsPaused;
    public bool IsStopped => _audioService.IsStopped;
    public int QueuedMedias => _mediasQueue.Count;

    public event EventHandler<MediaChangingEventArgs>? MediaChanging;

    public float Volume
    {
        get => _audioService.Volume;
        set => _audioService.Volume = value;
    }

    public async Task OpenMedia(IMediaInfo media, bool force = false)
    {
        if (!force && !AudioPlayerController.OpenMedia(media))
            return;

        MediaChanging?.Invoke(this,
                new MediaChangingEventArgs(CurrentMedia, media));

        if (!media.HasValidStreamUri())
        {
            var youtubeMedia = await _youtubeService.GetMediaAsync(media.Id!);
            if (youtubeMedia == null)
                return;
            _audioService.OpenMedia(youtubeMedia);
        }
        else
        {
            _audioService.OpenMedia(media);
        }

        if (AudioPlayerController.Play())
            _audioService.Play();
    }

    public async Task OpenPlaylist(Playlist playlist, bool force = false)
    {
        EnqueueMedias(playlist.Medias);
        if (TakeMedia() is { } media)
            await OpenMedia(media);
    }

    public void Play(bool force = false)
    {
        if (!force && !AudioPlayerController.Play())
            return;
        _audioService.Play();
    }

    public void Pause(bool force = false)
    {
        if (!force && !AudioPlayerController.Pause())
            return;
        _audioService.Pause();
    }

    public Task Skip(bool force = false)
    {
        if (!force && !AudioPlayerController.Skip())
            return Task.CompletedTask;
        if (TakeMedia() is { } media)
            return OpenMedia(media);
        return Task.CompletedTask;
    }

    public void Stop(bool force = false)
    {
        if (!force && !AudioPlayerController.Stop())
            return;
        _audioService.Stop();
    }

    public void SetPosition(in TimeSpan position, bool force = false)
    {
        if (!force && !AudioPlayerController.SetPosition(in position))
            return;
        _audioService.Position = position;
    }

    private IMediaInfo? TakeMedia()
    {
        if (_mediasQueue.Count == 0)
            return default;
        var media = _mediasQueue[Shuffle ? Random.Shared.Next(0, _mediasQueue.Count) : 0];
        _mediasQueue.RemoveAt(0);
        return media;
    }

    private void EnqueueMedias(IList<MediaInfo> infos)
    {
        if (infos.Count == 0)
            // TODO: Maybe log this ? as it should not happen
            return;

        _mediasQueue.AddRange(infos);
    }

    public IAudioPlayerController UpdateController(AudioPlayerControllerType audioPlayerControllerType)
    {
        AudioPlayerController?.OnRemoved();

        AudioPlayerController = audioPlayerControllerType switch
        {
            AudioPlayerControllerType.Remote => Ioc.Default.GetService<RemoteAudioPlayerController>()!,
            _ => Ioc.Default.GetService<LocalAudioPlayerController>()!
        };

        return AudioPlayerController;
    }

    private void OnPlaybackStopped(object? sender, EventArgs e)
    {
        Skip(); // Playback stopped, if there is a music in the queue lets play the next one.
    }
}