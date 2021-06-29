using System.ComponentModel;
using System.Windows;
using Nebula.ViewModel;
using Nebula.ViewModel.Dialogs;

namespace Nebula.View
{
    public class ViewModelsLocator
    {
        private MainWindowViewModel    _mainWindowViewModel;
        private MediaPlayerViewModel   _mediaPlayerViewModel;
        private MediasQueueViewModel   _mediasQueueViewModel;
        private OnlineSessionViewModel _onlineSessionViewModel;
        private SideBarViewModel       _sideBarViewModel;

        public ViewModelsLocator()
        {
            Instance = this;
        }

        public static ViewModelsLocator                      Instance                               { get; private set; }
        private       DependencyObject                       Dummy                                  { get; } = new();
        public        MainWindowViewModel                    MainWindowViewModel                    => _mainWindowViewModel ??= new MainWindowViewModel();
        public        MediaPlayerViewModel                   MediaPlayerViewModel                   => _mediaPlayerViewModel ??= new MediaPlayerViewModel();
        public        MediasQueueViewModel                   MediasQueueViewModel                   => _mediasQueueViewModel ??= new MediasQueueViewModel();
        public        OnlineSessionViewModel                 OnlineSessionViewModel                 => _onlineSessionViewModel ??= new OnlineSessionViewModel();
        public        SideBarViewModel                       SideBarViewModel                       => _sideBarViewModel ??= new SideBarViewModel();
        public        PlaylistViewModel                      PlaylistViewModel                      => new();
        public        SettingsViewModel                      SettingsViewModel                      => new();
        public        PlaylistImportDialogViewModel          PlaylistImportDialogViewModel          => new();
        public        PlaylistCreationDialogViewModel        PlaylistCreationDialogViewModel        => new();
        public        OnlineSessionJoinCreateDialogViewModel OnlineSessionJoinCreateDialogViewModel => new();
        public        MessageDialogViewModel                 MessageDialogViewModel                 => new();
        public        bool                                   IsDesignMode                           => DesignerProperties.GetIsInDesignMode(Dummy);
    }
}