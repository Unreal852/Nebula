using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nebula.Common.Audio;
using Nebula.Common.Medias;
using Nebula.Services.Contracts;

namespace Nebula.Desktop.ViewModel;

public sealed partial class AudioPlayerViewModel : ViewModelBase, IPositionChangedHandler
{
    private readonly IAudioPlayerService _audioPlayerService;
    private readonly IAudioService       _audioService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentMediaTitle), nameof(CurrentMediaAuthor), nameof(CurrentMediaDuration))]
    private IMediaInfo? _currentMedia;

    [ObservableProperty]
    private double _currentMediaPosition;

    [ObservableProperty]
    private bool _canForward;

    [ObservableProperty]
    private bool _canBackward;

    [ObservableProperty]
    private bool _isOpeningMedia;

    [ObservableProperty]
    private bool _shuffle;

    [ObservableProperty]
    private AudioServiceState _state = AudioServiceState.Stopped;

    [ObservableProperty]
    private double _volume = 25;

    public AudioPlayerViewModel(IAudioPlayerService audioPlayerService, IAudioService audioService)
    {
        _audioPlayerService = audioPlayerService;
        _audioService = audioService;
        _audioService.PositionChangedHandler = this;
        _audioPlayerService.MediaChanging += OnMediaChanging;
        _audioService.MediaChanged += OnMediaChanged;
        _audioService.StateChanged += OnStateChanged;
    }

    public string CurrentMediaTitle    => CurrentMedia?.GetFormattedTitle() ?? string.Empty;
    public string CurrentMediaAuthor   => CurrentMedia?.Author              ?? string.Empty;
    public double CurrentMediaDuration => CurrentMedia?.Duration            ?? 0;

    private void OnStateChanged(object? sender, Common.Audio.Events.AudioServiceStateChangedEventArgs e)
    {
        State = e.State;
    }

    private void OnMediaChanged(object? sender, Common.Audio.Events.MediaChangedEventArgs e)
    {
        CurrentMedia = e.NewMedia;
        CanForward = _audioPlayerService.QueuedMedias > 0;
        IsOpeningMedia = false;
    }

    private void OnMediaChanging(object? sender, Common.Audio.Events.MediaChangingEventArgs e)
    {
        IsOpeningMedia = true;
    }

    public void OnMediaChanged(IMediaInfo? oldMedia, IMediaInfo newMedia)
    {
        CurrentMedia = newMedia;
        CanForward = _audioPlayerService.QueuedMedias > 0;
        IsOpeningMedia = false;
    }

    public void OnPositionChanged(in TimeSpan position)
    {
        CurrentMediaPosition = position.TotalSeconds;
    }

    public void SetPosition(in TimeSpan span)
    {
        _audioPlayerService.SetPosition(span);
    }

    [RelayCommand]
    private void PlayPause()
    {
        if (_audioPlayerService.IsPaused)
            _audioPlayerService.Play();
        else if (_audioPlayerService.IsPlaying)
            _audioPlayerService.Pause();
    }

    [RelayCommand]
    private Task Skip()
    {
        return _audioPlayerService.Skip();
    }

    partial void OnShuffleChanged(bool value)
    {
        _audioPlayerService.Shuffle = value;
    }

    partial void OnVolumeChanged(double value)
    {
        _audioPlayerService.Volume = (float)value / 100;
    }
}