using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;

// ReSharper disable InconsistentNaming

namespace Nebula.Desktop.ViewModel;

public partial class ViewModelPageBase : ViewModelBase
{
    [ObservableProperty]
    protected ObservableCollection<object>? _subItems;

    [ObservableProperty]
    protected string _pageName = "Unknown";

    public Symbol PageIcon            { get; protected set; } = Symbol.Cancel;
    public bool   PageIsFooter        { get; protected set; } = false;
    public bool   PageIsDefault       { get; protected set; } = false;
    public bool   PageIsAlwaysVisible { get; protected set; } = true;

    public virtual void OnNavigatedTo()
    {
    }

    public virtual void OnNavigatingFrom()
    {
    }
}