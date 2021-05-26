using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HandyControl.Controls;
using HandyControl.Themes;
using HandyControl.Tools.Extension;
using Nebula.Media;
using Nebula.Media.Player;
using Nebula.Model;
using Nebula.MVVM;
using Nebula.MVVM.Commands;
using Nebula.Utils.Extensions;
using Nebula.View;
using Nebula.View.Views;
using Nebula.View.Views.Dialogs;
using Nebula.ViewModel.Dialogs;
using MessageBox = System.Windows.MessageBox;
using RelayCommand = HandyControl.Tools.Command.RelayCommand;

namespace Nebula.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        public static MainWindowViewModel Instance { get; private set; }

        private object _currentPage;
        private bool   _canSearch;

        public MainWindowViewModel()
        {
            Instance = this;
            SearchCommand = new AsyncRelayCommand<string>(Search);
            SwitchThemeCommand = new RelayCommand(SwitchTheme);
            CreatePlaylistCommand = new RelayCommand(CreatePlaylist);
            NavigatePlaylistCommand = new RelayCommand(NavigatePlaylist);
            SearchCommand.CanExecuteChanged += (_, _) => CanSearch = SearchCommand.CanExecute("");
        }

        public ICommand SearchCommand           { get; }
        public ICommand SwitchThemeCommand      { get; }
        public ICommand CreatePlaylistCommand   { get; }
        public ICommand NavigatePlaylistCommand { get; }


        public bool CanSearch
        {
            get => _canSearch;
            private set => Set(ref _canSearch, value);
        }

        public object CurrentPage
        {
            get => _currentPage;
            private set => Set(ref _currentPage, value);
        }

        private void SwitchTheme(object param)
        {
            if (ThemeManager.Current.ApplicationTheme == ApplicationTheme.Light)
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
            }
            else
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            }
        }

        private void CreatePlaylist(object obj)
        {
            //Dialog.Show<PlaylistCreationDialogView>().DataContext = new PlaylistCreationDialogViewModel();
            Dialog.Show(PlaylistImportationDialogViewModel.Cache.Container).DataContext = new PlaylistImportationDialogViewModel();
        }

        private async Task Search(string query)
        {
            SearchResultsView searchView = new SearchResultsView();
            SearchResultsViewModel searchResultsViewModel = searchView.DataContext as SearchResultsViewModel;
            Navigate(new NavigationInfo(searchView));
            if (searchResultsViewModel == null)
                throw new NullReferenceException("SearchResultsViewModel missing");
            await foreach (MediaInfo mediaInfo in NebulaClient.Providers.SearchMedias(query))
            {
                searchResultsViewModel.Medias.Add(mediaInfo);
                await Task.Delay(20);
            }
        }

        public void NavigatePlaylist(object param)
        {
            Navigate(NavigationInfo.Create(typeof(PlaylistView), NebulaClient.Playlists.Playlists[0]));
        }

        public void Navigate(object obj)
        {
            switch (obj)
            {
                case NavigationInfo info:
                {
                    UserControl userControl = info.Control ?? Activator.CreateInstance(info.Type) as UserControl;
                    if (userControl == null || (userControl.GetType() == CurrentPage?.GetType() && !info.NavigateIfSame))
                        return;
                    CurrentPage = userControl;
                    if (info.Parameter != null && userControl.DataContext is BaseViewModel viewModel)
                        viewModel.OnNavigating(info.Parameter);
                    break;
                }
                case Type type when Activator.CreateInstance(type) is UserControl control:
                    CurrentPage = control;
                    break;
            }
        }
    }
}