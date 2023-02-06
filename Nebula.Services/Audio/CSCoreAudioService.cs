using CSCore;
using CSCore.MediaFoundation;
using CSCore.SoundOut;
using Nebula.Common.Audio;
using Nebula.Common.Audio.Events;
using Nebula.Common.Extensions;
using Nebula.Common.Medias;
using Nebula.Services.Abstractions;
using Serilog;
using SerilogTimings.Extensions;

namespace Nebula.Services.Audio;

public sealed class CsCoreAudioService : IAudioService
{
    private readonly ILogger _logger;
    private float _volume = 0.2f;

    public event EventHandler<MediaChangedEventArgs>? MediaChanged;
    public event EventHandler<AudioServiceStateChangedEventArgs>? StateChanged;
    public event EventHandler<EventArgs>? PlaybackStopped;

    public CsCoreAudioService(ILogger logger)
    {
        _logger = logger.WithPrefix(nameof(CsCoreAudioService));
    }

    private ISoundOut? SoundOut { get; set; }
    private MediaFoundationDecoder? MediaDecoder { get; set; }
    private Task? WorkerTask { get; set; }
    private CancellationTokenSource? CancellationTokenSource { get; set; }
    public IPositionChangedHandler? PositionChangedHandler { get; set; }
    public IMediaInfo? CurrentMedia { get; private set; }
    public bool IsPlaying => SoundOut?.PlaybackState == PlaybackState.Playing;
    public bool IsPaused => SoundOut?.PlaybackState == PlaybackState.Paused;
    public bool IsStopped => SoundOut?.PlaybackState == PlaybackState.Stopped;

    public AudioServiceState State => SoundOut?.PlaybackState switch
    {
        PlaybackState.Playing => AudioServiceState.Playing,
        PlaybackState.Paused => AudioServiceState.Paused,
        PlaybackState.Stopped => AudioServiceState.Stopped,
        _ => AudioServiceState.Stopped
    };

    public TimeSpan Duration => MediaDecoder?.GetLength() ?? TimeSpan.Zero;

    public TimeSpan Position
    {
        get => MediaDecoder?.GetPosition() ?? TimeSpan.Zero;
        set => MediaDecoder?.SetPosition(value);
    }

    public float Volume
    {
        get => SoundOut?.Volume ?? 0;
        set
        {
            _volume = value;
            if(SoundOut != null)
                SoundOut.Volume = value;
        }
    }

    public void OpenMedia(IMediaInfo media)
    {
        if(media.StreamUri == null)
        {
            _logger.Information("Failed to open media ! The specified media stream uri is null");
            return;
        }

        using(_logger.TimeOperation("Opening media with id '{MediaId}'", media.Id ?? "NaN"))
        {
            Prepare(media.StreamUri);
            var oldMedia = CurrentMedia;
            CurrentMedia = media;
            if(CurrentMedia.Duration == 0) // TODO: Hacky way of settings the duration for local files
                CurrentMedia.Duration = MediaDecoder.GetLength().TotalSeconds;
            MediaChanged?.Invoke(this, new MediaChangedEventArgs(oldMedia, media));
        }
    }

    public void Play()
    {
        SoundOut?.Play();
        StateChanged?.Invoke(this, new AudioServiceStateChangedEventArgs(State));
    }

    public void Pause()
    {
        SoundOut?.Pause();
        StateChanged?.Invoke(this, new AudioServiceStateChangedEventArgs(State));
    }

    public void Stop()
    {
        SoundOut?.Stop();
    }

    public void Shutdown()
    {
        Stop();
        if(WorkerTask == null)
            return;
        _logger.Information("Stopping service...");
        CancellationTokenSource?.Cancel();
        Task.WaitAny(WorkerTask!);
    }

    private void Prepare(string uri)
    {
        if(SoundOut != null)
        {
            SoundOut.Stopped -= OnPlaybackStopped;
            SoundOut.Dispose();
            SoundOut = null;
        }

        if(MediaDecoder != null)
        {
            MediaDecoder.Dispose();
            MediaDecoder = null;
        }

        InitializeWorkerTask();
        MediaDecoder = new MediaFoundationDecoder(uri);
        SoundOut = new WasapiOut();
        SoundOut.Stopped += OnPlaybackStopped;
        SoundOut.Initialize(MediaDecoder);
        SoundOut.Volume = _volume;
    }

    private void InitializeWorkerTask()
    {
        if(WorkerTask != null)
            return;
        _logger.Information("Starting service...");
        CancellationTokenSource = new CancellationTokenSource();
        var token = CancellationTokenSource.Token;
        WorkerTask = Task.Run(() => ServiceWorker(in token), token);
    }

    private void OnPlaybackStopped(object? sender, PlaybackStoppedEventArgs e)
    {
        PlaybackStopped?.Invoke(this, EventArgs.Empty);
        StateChanged?.Invoke(this, new AudioServiceStateChangedEventArgs(State));
    }

    private void ServiceWorker(in CancellationToken token)
    {
        _logger.Information("Service is now running !");
        var lastPos = default(TimeSpan);
        try
        {
            while(true)
            {
                token.ThrowIfCancellationRequested();

                if(State == AudioServiceState.Playing && lastPos != Position)
                {
                    lastPos = Position;
                    PositionChangedHandler?.OnPositionChanged(in lastPos);
                }

                Thread.Sleep(50);
            }
        }
        catch(Exception e)
        {
            if(e is not OperationCanceledException) _logger.Error(e, "Service error");
        }
        finally
        {
            _logger.Information("Service has been shut down !");
        }
    }
}