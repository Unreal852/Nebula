using System.ComponentModel;
using System.Windows;
using Nebula.ViewModel;
using Nebula.ViewModel.Dialogs;

namespace Nebula.View
{
    public class ViewModelsLocator
    {
        private MainWindowViewModel  _mainWindowViewModel;
        private MediaPlayerViewModel _mediaPlayerViewModel;
        private MediasQueueViewModel _mediasQueueViewModel;

        private DependencyObject               Dummy                                { get; } = new();
        public  MainWindowViewModel            MainWindowViewModel                  => _mainWindowViewModel ??= new MainWindowViewModel();
        public  MediaPlayerViewModel           MediaPlayerViewModel                 => _mediaPlayerViewModel ??= new MediaPlayerViewModel();
        public  MediasQueueViewModel           MediasQueueViewModel                 => _mediasQueueViewModel ??= new MediasQueueViewModel();
        public  LocalPlaylistSelectorViewModel LocalPlaylistSelectorDialogViewModel => new();
        public  bool                           IsDesignMode                         => DesignerProperties.GetIsInDesignMode(Dummy);
    }
}