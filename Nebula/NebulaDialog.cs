using System.Windows;
using System.Windows.Media;
using HandyControl.Controls;
using Nebula.View.Views.Dialogs;
using Nebula.ViewModel.Dialogs;

namespace Nebula
{
    public static class NebulaDialog
    {
        public static Dialog ShowError(string message, string title = "dialog_title_error")
        {
            return ShowMessage(message, title, MessageDialog.Ok, "#D00606");
        }

        public static Dialog ShowInfo(string message, string title = "dialog_title_info")
        {
            return ShowMessage(message, title, MessageDialog.Ok, "#349fff");
        }

        public static Dialog ShowInfoCancelConfirm(string message, string title = "dialog_title_info")
        {
            return ShowMessage(message, title, MessageDialog.CancelConfirm, "#349fff");
        }

        public static Dialog ShowInfoNoYes(string message, string title = "dialog_title_info")
        {
            return ShowMessage(message, title, MessageDialog.NoYes, "#349fff");
        }

        public static Dialog ShowWarning(string message, string title = "dialog_title_warning")
        {
            return ShowMessage(message, title, MessageDialog.Ok, "#Df6316");
        }

        public static Dialog ShowWarningCancelConfirm(string message, string title = "dialog_title_warning")
        {
            return ShowMessage(message, title, MessageDialog.CancelConfirm, "#Df6316");
        }

        public static Dialog ShowWarningNoYes(string message, string title = "dialog_title_warning")
        {
            return ShowMessage(message, title, MessageDialog.NoYes, "#Df6316");
        }

        public static Dialog ShowDialog<TDialog, TViewModel>(string token = "") where TDialog : FrameworkElement, new() where TViewModel : BaseDialogViewModel, new()
        {
            var dialog = Dialog.Show<TDialog>(token);
            TViewModel viewModel = new() {Dialog = dialog};
            dialog.DataContext = viewModel;
            return dialog;
        }

        public static Dialog ShowDialog(object content, string token = "")
        {
            var dialog = Dialog.Show(content, token);
            if (dialog.Content is FrameworkElement {DataContext: BaseDialogViewModel baseDialogViewModel})
                baseDialogViewModel.Dialog = dialog;
            return dialog;
        }

        public static Dialog ShowDialog<TDialog>(string token = "") where TDialog : FrameworkElement, new()
        {
            var dialog = Dialog.Show<TDialog>(token);
            if (dialog.Content is TDialog {DataContext: BaseDialogViewModel baseDialogViewModel})
                baseDialogViewModel.Dialog = dialog;
            return dialog;
        }

        public static Dialog ShowMessage(string message, string title, MessageDialog messageDialog = MessageDialog.CancelConfirm, string titleHexColor = "")
        {
            var dialog = ShowDialog<MessageDialogView>();
            if (dialog.Content is MessageDialogView {DataContext: MessageDialogViewModel messageDialogViewModel})
            {
                messageDialogViewModel.Dialog = dialog;
                messageDialogViewModel.Title = NebulaClient.GetLang(title);
                messageDialogViewModel.Message = NebulaClient.GetLang(message);
                messageDialogViewModel.DialogType = messageDialog;
                if (!string.IsNullOrWhiteSpace(titleHexColor))
                    messageDialogViewModel.TitleBrush = new SolidColorBrush((Color) ColorConverter.ConvertFromString(titleHexColor));
            }

            return dialog;
        }
    }
}