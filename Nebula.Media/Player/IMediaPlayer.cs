using System;
using System.Threading.Tasks;
using Nebula.Media.Player.Events;

namespace Nebula.Media.Player
{
    public interface IMediaPlayer
    {
        bool       IsIdle          { get; }
        bool       IsPlaying       { get; }
        bool       IsPaused        { get; }
        bool       IsMuted         { get; set; }
        bool       Repeat          { get; set; }
        bool       Shuffle         { get; set; }
        int        Volume          { get; set; }
        IMediaInfo CurrentMedia    { get; }
        IPlaylist  CurrentPlaylist { get; }
        TimeSpan   Position        { get; }
        TimeSpan   Duration        { get; }

        event EventHandler<PlayerMediaChangedEventArgs>    MediaChanged;
        event EventHandler<PlayerPlaylistChangedEventArgs> PlaylistChanged;
        event EventHandler<PlayerStateChangedEventArgs>    StateChanged;
        event EventHandler<PlayerVolumeChangedEventArgs>   VolumeChanged;
        event EventHandler<PlayerMuteChangedEventArgs>     MuteChanged;
        event EventHandler<PlayerShuffleChangedEventArgs>  ShuffleChanged;
        event EventHandler<PlayerRepeatChangedEventArgs>   RepeatChanged;
        event EventHandler<PlayerPositionChangedEventArgs> PositionChanged;

        Task OpenMedia(IMediaInfo mediaInfo, bool byUser = true, bool fromRemote = false);
        Task OpenPlaylist(IPlaylist playlist);
        Task Forward(bool byUser);
        Task Backward(bool byUser);
        void Play(bool fromRemote = false);
        void Pause(bool fromRemote = false);
        void Resume(bool fromRemote = false);
        void Stop(bool byUser = false);
        void SetPosition(double position, bool fromRemote);
    }
}