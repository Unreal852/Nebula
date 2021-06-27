using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using HandyControl.Themes;
using LiteMVVM;
using LiteMVVM.Command;
using LiteMVVM.Navigation;
using Nebula.Model;
using Nebula.View;
using Nebula.View.Views;
using Nebula.View.Views.Dialogs;

namespace Nebula.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        public static MainWindowViewModel Instance { get; private set; }

        private object _currentPage;
        private bool   _showQueue;
        private bool   _canSearch;

        public MainWindowViewModel()
        {
            Instance = this;
            SearchCommand = new AsyncRelayCommand<string>(Search);
            ShowSettingsCommand = new RelayCommand(() => Navigate(NavigationInfo.Create(typeof(SettingsView), null, false)));
            ShowQueueCommand = new RelayCommand(() => ShowQueue = !ShowQueue);
            Messenger.Subscribe<NavigationInfo>((_, o) => Navigate(o));
            SearchCommand.CanExecuteChanged += (_, _) => CanSearch = SearchCommand.CanExecute("");
        }

        public ICommand SearchCommand       { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand ShowQueueCommand    { get; }

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

        public bool ShowQueue
        {
            get => _showQueue;
            set => Set(ref _showQueue, value);
        }

        public bool IsCurrentPage(Type pageType)
        {
            return CurrentPage?.GetType() == pageType;
        }

        private async Task Search(string query)
        {
            SearchResultsView searchView = new SearchResultsView();
            SearchResultsViewModel searchResultsViewModel = searchView.DataContext as SearchResultsViewModel;
            Navigate(new NavigationInfo(searchView, null, true));
            if (searchResultsViewModel == null)
                throw new NullReferenceException("SearchResultsViewModel missing");
            await foreach (MediaInfo mediaInfo in NebulaClient.Providers.SearchMedias(query))
            {
                searchResultsViewModel.Medias.Add(mediaInfo);
                await Task.Delay(20);
            }
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
                    if (CurrentPage is UserControl {DataContext: INavigable curNavigable})
                        curNavigable.OnLeft();
                    CurrentPage = userControl;
                    if (info.Parameter != null && userControl.DataContext is INavigable navigable)
                        navigable.OnNavigated(info.Parameter);
                    break;
                }
                case Type type when Activator.CreateInstance(type) is UserControl control:
                {
                    CurrentPage = control;
                    if (control.DataContext is INavigable navigable)
                        navigable.OnNavigated(null);
                    break;
                }
                case "exit":
                {
                    if (CurrentPage is UserControl {DataContext: INavigable curNavigable})
                        curNavigable.OnLeft();
                    break;
                }
            }
        }
    }
}