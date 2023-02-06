using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;

namespace Nebula.Desktop.View.Dialogs;

public static class Dialog
{
    public static ContentDialog CreateDialog<TContent>(string title, string primaryButtonText,
                                                       string secondaryButtonText, string closeButtonText)
            where TContent : UserControl, new()
    {
        return new ContentDialog
        {
                Title = title,
                PrimaryButtonText = primaryButtonText,
                SecondaryButtonText = secondaryButtonText,
                CloseButtonText = closeButtonText,
                Content = new TContent()
        };
    }

    public static ContentDialog CreateDialog<TContent>(string title, string primaryButtonText, string closeButtonText)
            where TContent : UserControl, new()
    {
        return new ContentDialog
        {
                Title = title,
                PrimaryButtonText = primaryButtonText,
                CloseButtonText = closeButtonText,
                Content = new TContent()
        };
    }

    public static ContentDialog CreateDialog<TContent, TViewModel>(string title, string primaryButtonText,
                                                                   string closeButtonText)
            where TContent : UserControl, new() where TViewModel : ObservableObject, new()
    {
        ContentDialog contentDialog = CreateDialog<TContent>(title, primaryButtonText, closeButtonText);
        if (contentDialog is { Content: UserControl control })
            control.DataContext = new TViewModel();
        return contentDialog;
    }

    public static Task<ContentDialogResult> ShowDialog<TContent, TViewModel>(
    string title, string primaryButtonText, string closeButtonText)
            where TContent : UserControl, new() where TViewModel : ObservableObject, new()
    {
        ContentDialog contentDialog = CreateDialog<TContent, TViewModel>(title, primaryButtonText, closeButtonText);
        return contentDialog.ShowAsync();
    }

    public static Task<ContentDialogResult> ShowDialog<TContent>(string title, string primaryButtonText,
                                                                 string closeButtonText)
            where TContent : UserControl, new()
    {
        ContentDialog contentDialog = CreateDialog<TContent>(title, primaryButtonText, closeButtonText);
        return contentDialog.ShowAsync();
    }
}