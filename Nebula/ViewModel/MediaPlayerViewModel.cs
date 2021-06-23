using System;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteMVVM;
using LiteMVVM.Command;
using Nebula.Core.Player;
using Nebula.Model;

namespace Nebula.ViewModel
{
    public class MediaPlayerViewModel : BaseViewModel
    {
        public static MediaPlayerViewModel Instance { get; private set; }

        public MediaPlayerViewModel()
        {
            Instance = this;
            ForwardCommand = new AsyncRelayCommand(GoForward);
            BackwardCommand = new AsyncRelayCommand(GoBackward);
            PlayPauseCommand = new RelayCommand(() => IsPlaying = !MediaPlayer.IsPlaying);
            MediaPlayer.MediaChanged += (_, _) =>
            {
                OnPropertyChanged(nameof(CurrentMedia));
                OnPropertyChanged(nameof(Duration));
            };
            MediaPlayer.StateChanged += (_, _) => OnPropertyChanged(nameof(IsPlaying));
            MediaPlayer.PositionChanged += (_, _) => OnPropertyChanged(nameof(Position));
            MediaPlayer.VolumeChanged += (_, _) => OnPropertyChanged(nameof(Volume));
            MediaPlayer.ShuffleChanged += (_, _) => OnPropertyChanged(nameof(Shuffle));
            MediaPlayer.RepeatChanged += (_, _) => OnPropertyChanged(nameof(Repeat));
        }

        public ICommand ForwardCommand   { get; }
        public ICommand BackwardCommand  { get; }
        public ICommand PlayPauseCommand { get; }

        public NAudioPlayer MediaPlayer => NebulaClient.MediaPlayer;

        public MediaInfo CurrentMedia => MediaPlayer.CurrentMedia;
        public TimeSpan  Duration     => MediaPlayer.Duration;

        public bool IsPlaying
        {
            get => MediaPlayer.IsPlaying;
            set
            {
                if (IsPlaying)
                    MediaPlayer.Pause();
                else
                    MediaPlayer.Resume();
            }
        }

        public bool Shuffle
        {
            get => MediaPlayer.Shuffle;
            set
            {
                MediaPlayer.Shuffle = value;
                OnPropertyChanged();
            }
        }

        public bool Repeat
        {
            get => MediaPlayer.Repeat;
            set
            {
                MediaPlayer.Repeat = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan Position
        {
            get => MediaPlayer.Position;
            set => MediaPlayer.SetPosition(value.TotalSeconds, false);
        }

        public double Volume
        {
            get => MediaPlayer.Volume;
            set => MediaPlayer.Volume = Convert.ToInt32(value);
        }

        private async Task GoForward()  => await MediaPlayer.Forward(true);
        private async Task GoBackward() => await MediaPlayer.Backward(true);
    }
}