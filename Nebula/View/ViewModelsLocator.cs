using System.ComponentModel;
using System.Windows;
using Nebula.ViewModel;

namespace Nebula.View
{
    public class ViewModelsLocator
    {
        public static ViewModelsLocator Instance { get; private set; }

        public ViewModelsLocator()
        {
            Instance = this;
        }

        private MainWindowViewModel    _mainWindowViewModel;
        private MediaPlayerViewModel   _mediaPlayerViewModel;
        private MediasQueueViewModel   _mediasQueueViewModel;
        private OnlineSessionViewModel _onlineSessionViewModel;

        private DependencyObject       Dummy                  { get; } = new();
        public  MainWindowViewModel    MainWindowViewModel    => _mainWindowViewModel ??= new MainWindowViewModel();
        public  MediaPlayerViewModel   MediaPlayerViewModel   => _mediaPlayerViewModel ??= new MediaPlayerViewModel();
        public  MediasQueueViewModel   MediasQueueViewModel   => _mediasQueueViewModel ??= new MediasQueueViewModel();
        public  OnlineSessionViewModel OnlineSessionViewModel => _onlineSessionViewModel ??= new OnlineSessionViewModel();
        public  bool                   IsDesignMode           => DesignerProperties.GetIsInDesignMode(Dummy);
    }
}