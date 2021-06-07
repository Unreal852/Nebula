using System.ComponentModel;
using System.Windows;
using Nebula.ViewModel;

namespace Nebula.View
{
    public class ViewModelsLocator
    {
        private MainWindowViewModel  _mainWindowViewModel;
        private MediaPlayerViewModel _mediaPlayerViewModel;

        private DependencyObject     Dummy                { get; } = new();
        public  MainWindowViewModel  MainWindowViewModel  => _mainWindowViewModel ??= new MainWindowViewModel();
        public  MediaPlayerViewModel MediaPlayerViewModel => _mediaPlayerViewModel ??= new MediaPlayerViewModel();
        public  bool                 IsDesignMode         => DesignerProperties.GetIsInDesignMode(Dummy);
    }
}