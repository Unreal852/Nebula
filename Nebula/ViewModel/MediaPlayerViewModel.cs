using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Nebula.Media;
using Nebula.Media.Player;
using Nebula.MVVM;
using Nebula.MVVM.Commands;

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
            MediaPlayer.MediaChanged += (_, _) =>
            {
                OnPropertyChanged(nameof(CurrentMedia));
                OnPropertyChanged(nameof(Duration));
            };
            MediaPlayer.StateChanged += (_, _) => OnPropertyChanged(nameof(IsPlaying));
            MediaPlayer.PositionChanged += (_, _) => OnPropertyChanged(nameof(Position));
            MediaPlayer.VolumeChanged += (_, _) => OnPropertyChanged(nameof(Volume));
        }

        public ICommand ForwardCommand  { get; }
        public ICommand BackwardCommand { get; }

        public IMediaPlayer MediaPlayer => NebulaClient.MediaPlayer;

        public IMediaInfo CurrentMedia => MediaPlayer.CurrentMedia;
        public TimeSpan   Duration     => MediaPlayer.Duration;

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

        private async Task GoForward(object param)  => await MediaPlayer.Forward(true);
        private async Task GoBackward(object param) => await MediaPlayer.Backward(true);
    }
}