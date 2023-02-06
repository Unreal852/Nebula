using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using Nebula.Common.Audio;
using Nebula.Common.Extensions;
using Nebula.Common.Medias;
using Nebula.Services.Abstractions;
using Serilog;
using SerilogTimings.Extensions;

namespace Nebula.Services.Audio;

public sealed class CsCoreAudioService : IAudioService
{
    private readonly ILogger                     _logger;
    private          IAudioServiceEventsHandler? _eventsHandler;
    private          float                       _volume = 0.2f;

    public CsCoreAudioService(ILogger logger)
    {
        _logger = logger.WithPrefix(nameof(CsCoreAudioService));
    }

    private ISoundOut?               SoundOut                { get; set; }
    private IWaveSource?             WaveSource              { get; set; }
    private Task?                    WorkerTask              { get; set; }
    private CancellationTokenSource? CancellationTokenSource { get; set; }
    public  MediaInfos?              CurrentMedia            { get; private set; }
    public  bool                     IsPlaying               => SoundOut?.PlaybackState == PlaybackState.Playing;
    public  bool                     IsPaused                => SoundOut?.PlaybackState == PlaybackState.Paused;
    public  bool                     IsStopped               => SoundOut?.PlaybackState == PlaybackState.Stopped;

    public AudioServiceState State => SoundOut?.PlaybackState switch
    {
            PlaybackState.Playing => AudioServiceState.Playing,
            PlaybackState.Paused  => AudioServiceState.Paused,
            PlaybackState.Stopped => AudioServiceState.Stopped,
            _                     => AudioServiceState.Stopped
    };

    public TimeSpan Duration => WaveSource?.GetLength() ?? TimeSpan.Zero;

    public TimeSpan Position
    {
        get => WaveSource?.GetPosition() ?? TimeSpan.Zero;
        set => WaveSource?.SetPosition(value);
    }

    public float Volume
    {
        get => SoundOut?.Volume ?? 0;
        set
        {
            _volume = value;
            if (SoundOut != null)
                SoundOut.Volume = value;
        }
    }

    public void OpenMedia(MediaInfos media)
    {
        if (media.StreamUri == null)
        {
            _logger.Information("Failed to open media ! The specified media stream uri is null");
            return;
        }

        using (_logger.TimeOperation("Opening media with stream uri '{MediaId}'", media.Id ?? "NaN"))
        {
            Prepare(media.StreamUri);
            MediaInfos? oldMedia = CurrentMedia;
            CurrentMedia = media;
            _eventsHandler?.OnMediaChanged(oldMedia, media);
        }
    }

    public void Play()
    {
        SoundOut?.Play();
        _eventsHandler?.OnStateChanged(State);
    }

    public void Pause()
    {
        SoundOut?.Pause();
        _eventsHandler?.OnStateChanged(State);
    }

    public void Stop()
    {
        SoundOut?.Stop();
    }

    public void Shutdown()
    {
        Stop();
        if (WorkerTask == null)
            return;
        _logger.Information("Stopping service...");
        CancellationTokenSource?.Cancel();
        Task.WaitAny(WorkerTask!);
    }

    public void SubscribeEvents(IAudioServiceEventsHandler eventsHandler)
    {
        _eventsHandler = eventsHandler; // TODO: Allow multiple subscribers
    }

    private void Prepare(Uri uri)
    {
        if (SoundOut != null)
        {
            SoundOut.Stopped -= OnPlaybackStopped;
            SoundOut.Dispose();
            SoundOut = null;
        }

        if (WaveSource != null)
        {
            WaveSource.Dispose();
            WaveSource = null;
        }

        InitializeWorkerTask();
        WaveSource = CodecFactory.Instance.GetCodec(uri);
        SoundOut = new WasapiOut();
        SoundOut.Stopped += OnPlaybackStopped;
        SoundOut.Initialize(WaveSource);
        SoundOut.Volume = _volume;
    }

    private void InitializeWorkerTask()
    {
        if (WorkerTask != null)
            return;
        _logger.Information("Starting service...");
        CancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = CancellationTokenSource.Token;
        WorkerTask = Task.Run(() => ServiceWorker(in token), token);
    }

    private void OnPlaybackStopped(object? sender, PlaybackStoppedEventArgs e)
    {
        if (_eventsHandler is { })
        {
            _eventsHandler.OnPlaybackStopped();
            _eventsHandler.OnStateChanged(AudioServiceState.Stopped);
        }
    }

    private void ServiceWorker(in CancellationToken token)
    {
        _logger.Information("Service is now running !");
        TimeSpan lastPos = default;
        try
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();

                if (State == AudioServiceState.Playing && lastPos != Position)
                {
                    lastPos = Position;
                    _eventsHandler?.OnPositionChanged(in lastPos);
                }

                Thread.Sleep(50);
            }
        }
        catch (Exception e)
        {
            if (e is not OperationCanceledException) _logger.Error(e, "Service error");
        }
        finally
        {
            _logger.Information("Service has been shut down !");
        }
    }
}