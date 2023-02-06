using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Nebula.Desktop.ViewModel.Messages;

namespace Nebula.Desktop.ViewModel;

public sealed partial class MainViewModel : ViewModelBase, IRecipient<PageChangeMessage>
{
    [ObservableProperty]
    private ViewModelBase? _currentPage;

    [ObservableProperty]
    private ObservableCollection<ViewModelBase> _footerPages = default!;

    [ObservableProperty]
    private ObservableCollection<ViewModelBase> _menuPages = default!;

    public MainViewModel(IEnumerable<ViewModelPageBase> viewModelBases)
    {
        var menuPages = new ObservableCollection<ViewModelBase>();
        var footerPages = new ObservableCollection<ViewModelBase>();
        foreach (var viewModelBase in viewModelBases)
        {
            if (!viewModelBase.PageIsAlwaysVisible)
                continue;
            if (viewModelBase.PageIsFooter)
                footerPages.Add(viewModelBase);
            else
                menuPages.Add(viewModelBase);
            if (viewModelBase.PageIsDefault)
                CurrentPage = viewModelBase;
        }

        MenuPages = menuPages;
        FooterPages = footerPages;

        StrongReferenceMessenger.Default.RegisterAll(this);
    }

    public void Receive(PageChangeMessage message)
    {
        CurrentPage = message.Value;
    }

    partial void OnCurrentPageChanging(ViewModelBase? value)
    {
        if (CurrentPage is ViewModelPageBase page)
        {
            page.OnNavigatingFrom();
        }
    }

    partial void OnCurrentPageChanged(ViewModelBase? value)
    {
        if (value is ViewModelPageBase page)
        {
            page.OnNavigatedTo();
        }
    }
}