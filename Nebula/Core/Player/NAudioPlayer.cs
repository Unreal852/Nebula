using System;
using System.Threading.Tasks;
using HandyControl.Controls;
using NAudio.Wave;
using Nebula.Media;
using Nebula.Media.Player;
using Nebula.Media.Player.Events;
using Nebula.Model;

#pragma warning disable 4014

namespace Nebula.Core.Player
{
    public class NAudioPlayer : IMediaPlayer
    {
        private IMediaInfo _currentMedia;
        private IPlaylist  _currentPlaylist;
        private TimeSpan   _lastPosition = TimeSpan.Zero;
        private bool       _shuffle;
        private bool       _repeat;
        private bool       _stoppedByUSer;
        private bool       _isMuted;
        private int        _volume;
        private int        _volumeBeforeMute;

        public NAudioPlayer()
        {
            NebulaClient.Tick += OnAppTick;
            _volume = 50;
        }

        public  bool       IsIdle     => State == PlayerState.Idle;
        public  bool       IsPlaying  => State == PlayerState.Playing;
        public  bool       IsPaused   => State == PlayerState.Paused;
        public  TimeSpan   Position   => SoundOut.Reader?.CurrentTime ?? TimeSpan.Zero;
        public  TimeSpan   Duration   => SoundOut.Reader?.TotalTime ?? TimeSpan.Zero;
        public  MediaQueue MediaQueue { get; } = new();
        private SoundOut   SoundOut   { get; } = new();

        public PlayerState State
        {
            get
            {
                PlaybackState playbackState = SoundOut.Out?.PlaybackState ?? PlaybackState.Stopped;
                switch (playbackState)
                {
                    case PlaybackState.Playing:
                        return PlayerState.Playing;
                    case PlaybackState.Paused:
                        return PlayerState.Paused;
                    default:
                        return PlayerState.Idle;
                }
            }
        }

        public IMediaInfo CurrentMedia
        {
            get => _currentMedia;
            private set
            {
                IMediaInfo oldMedia = _currentMedia;
                _currentMedia = value;
                MediaChanged?.Invoke(this, new PlayerMediaChangedEventArgs(null, oldMedia, value));
            }
        }

        public IPlaylist CurrentPlaylist
        {
            get => _currentPlaylist;
            private set
            {
                IPlaylist oldPlaylist = _currentPlaylist;
                _currentPlaylist = value;
                PlaylistChanged?.Invoke(this, new PlayerPlaylistChangedEventArgs(oldPlaylist, value));
            }
        }

        public bool Repeat
        {
            get => _repeat;
            set
            {
                if (_repeat == value)
                    return;
                _repeat = value;
                RepeatChanged?.Invoke(this, new PlayerRepeatChangedEventArgs(value));
            }
        }

        public bool Shuffle
        {
            get => _shuffle;
            set
            {
                if (_shuffle == value)
                    return;
                _shuffle = value;
                ShuffleChanged?.Invoke(this, new PlayerShuffleChangedEventArgs(value));
            }
        }

        public bool IsMuted
        {
            get => _isMuted;
            set
            {
                if (value)
                    _volumeBeforeMute = Volume;
                Volume = value ? 0 : _volumeBeforeMute;
                _isMuted = value;
                MuteChanged?.Invoke(this, new PlayerMuteChangedEventArgs(value));
            }
        }

        public int Volume
        {
            get => _volume;
            set
            {
                int oldVolume = _volume;
                _volume = value < 0 ? 0 : value > 100 ? 100 : value;
                if (SoundOut.IsReady)
                    SoundOut.Out.Volume = Math.Min(1.0f, Math.Max(value / 100f, 0f));
                VolumeChanged?.Invoke(this, new PlayerVolumeChangedEventArgs(oldVolume, value));
            }
        }

        public event EventHandler<PlayerMediaChangedEventArgs>    MediaChanged;
        public event EventHandler<PlayerPlaylistChangedEventArgs> PlaylistChanged;
        public event EventHandler<PlayerStateChangedEventArgs>    StateChanged;
        public event EventHandler<PlayerVolumeChangedEventArgs>   VolumeChanged;
        public event EventHandler<PlayerMuteChangedEventArgs>     MuteChanged;
        public event EventHandler<PlayerShuffleChangedEventArgs>  ShuffleChanged;
        public event EventHandler<PlayerRepeatChangedEventArgs>   RepeatChanged;
        public event EventHandler<PlayerPositionChangedEventArgs> PositionChanged;

        public async Task OpenMedia(IMediaInfo mediaInfo, bool byUser = true, bool fromRemote = false)
        {
            if (mediaInfo == null)
                return;
            try
            {
                Stop(byUser);
                SoundOut.Prepare(await mediaInfo.GetAudioStreamUri());
                if (SoundOut.IsReady)
                    SoundOut.Out.Volume = Math.Min(1.0f, Math.Max(Volume / 100f, 0f));
                SoundOut.Out.PlaybackStopped -= OnMediaStopped;
                SoundOut.Out.PlaybackStopped += OnMediaStopped;
            }
            catch (Exception ex)
            {
                Growl.Error(ex.StackTrace);
                StateChanged?.Invoke(this, new PlayerStateChangedEventArgs(State));
                return;
            }

            CurrentMedia = mediaInfo;
            Play();
        }

        public async Task OpenPlaylist(IPlaylist playlist)
        {
            if (playlist == null)
                return;
            CurrentPlaylist = playlist;
            MediaQueue.Enqueue((Playlist) playlist);
            await OpenMedia(MediaQueue.Dequeue(Shuffle));
        }

        public async Task Forward(bool byUser)
        {
            await OpenMedia(MediaQueue.Dequeue(Shuffle), byUser);
        }

        public async Task Backward(bool byUser)
        {
            await OpenMedia(MediaQueue.RewindDequeue(), byUser);
        }

        public void Play(bool fromRemote = false)
        {
            if (IsPaused || !SoundOut.IsReady)
                return;
            _stoppedByUSer = false;
            SoundOut.Out.Play();
            StateChanged?.Invoke(this, new PlayerStateChangedEventArgs(State));
        }

        public void Pause(bool fromRemote = false)
        {
            if (IsIdle || IsPaused || !SoundOut.IsReady)
                return;
            SoundOut.Out.Pause();
            StateChanged?.Invoke(this, new PlayerStateChangedEventArgs(State));
        }

        public void Resume(bool fromRemote = false)
        {
            if (IsIdle || IsPlaying || !SoundOut.IsReady)
                return;
            SoundOut.Out.Play();
            StateChanged?.Invoke(this, new PlayerStateChangedEventArgs(State));
        }

        public void Stop(bool byUser = false)
        {
            if (IsIdle || !SoundOut.IsReady)
                return;
            _stoppedByUSer = byUser;
            SoundOut.Out.Stop();
            StateChanged?.Invoke(this, new PlayerStateChangedEventArgs(State));
        }

        public void SetPosition(double position, bool fromRemote)
        {
            if (!SoundOut.IsReady)
                return;
            SoundOut.Reader.CurrentTime = TimeSpan.FromSeconds(position);
        }

        private void OnMediaStopped(object sender, StoppedEventArgs e)
        {
            if (!_stoppedByUSer)
            {
                if (Repeat)
                    OpenMedia(CurrentMedia);
                if (!MediaQueue.IsEmpty)
                    Forward(true);
            }
        }

        private void OnAppTick(object sender, EventArgs e)
        {
            if (!SoundOut.IsReady)
                return;
            if (Math.Abs(_lastPosition.TotalSeconds - Position.TotalSeconds) >= 0.5)
            {
                PositionChanged?.Invoke(this, new PlayerPositionChangedEventArgs(Position, Duration));
                _lastPosition = Position;
            }
        }
    }
}