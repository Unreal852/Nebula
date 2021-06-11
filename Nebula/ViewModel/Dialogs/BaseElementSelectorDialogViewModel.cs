using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using HandyControl.Tools.Extension;
using LiteMVVM.Command;
using Nebula.Utils.Collections.Paging;

namespace Nebula.ViewModel.Dialogs
{
    public abstract class BaseElementSelectorDialogViewModel<T> : BaseDialogViewModel, IDialogResultable<T>
    {
        private bool         _searchBarVisible;
        private DataTemplate _itemsTemplate;

        protected BaseElementSelectorDialogViewModel()
        {
            Pager = new ObservableFilterPager<T>(null);
            SearchCommand = new AsyncRelayCommand<string>(Search);
            TitleBrush = new SolidColorBrush(Colors.DodgerBlue);
        }

        public ObservableFilterPager<T> Pager           { get; }
        public ICommand                 SearchCommand   { get; }
        public T                        Result          { get; set; }
        public Action                   CloseAction     { get; set; }
        public object                   SelectedElement { get; set; }

        public DataTemplate ItemsTemplate
        {
            get => _itemsTemplate;
            set => Set(ref _itemsTemplate, value);
        }

        public bool SearchBarVisible
        {
            get => _searchBarVisible;
            set => Set(ref _searchBarVisible, value);
        }

        protected abstract Task Search(string value);
    }
}