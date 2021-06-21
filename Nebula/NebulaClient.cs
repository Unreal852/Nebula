using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using HandyControl.Controls;
using Nebula.Core;
using Nebula.Core.Database;
using Nebula.Core.Discord;
using Nebula.Core.Player;
using Nebula.Core.Providers;
using Nebula.View.Views.Dialogs;
using Nebula.ViewModel.Dialogs;

namespace Nebula
{
    public static class NebulaClient
    {
        static NebulaClient()
        {
            Providers = new ProvidersManager();
            Playlists = new PlaylistsManager();
            Database = new NebulaDatabase();
            // Order does not matter below 
            Discord = new Discord(740292732794306690, (ulong) CreateFlags.Default);
            Discord.SetLogHook(LogLevel.Debug, (_, message) => Debug.Print(message));
            Discord.SetLogHook(LogLevel.Error, (_, message) => Debug.Print(message));
            Discord.SetLogHook(LogLevel.Warn, (_, message) => Debug.Print(message));
            MediaPlayer = new NAudioPlayer();
            CancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => AppTick(CancellationTokenSource.Token, 500));
        }

        public static   ProvidersManager        Providers               { get; }
        public static   NAudioPlayer            MediaPlayer             { get; }
        public static   NebulaDatabase          Database                { get; }
        public static   PlaylistsManager        Playlists               { get; }
        public static   Discord                 Discord                 { get; }
        internal static CancellationTokenSource CancellationTokenSource { get; }

        public static event EventHandler Tick;

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
                messageDialogViewModel.Title = GetLang(title);
                messageDialogViewModel.Message = GetLang(message);
                messageDialogViewModel.DialogType = messageDialog;
                if (!string.IsNullOrWhiteSpace(titleHexColor))
                    messageDialogViewModel.TitleBrush = new SolidColorBrush((Color) ColorConverter.ConvertFromString(titleHexColor));
            }

            return dialog;
        }

        public static string GetLang(string key, params object[] format)
        {
            if (format is not {Length: > 0})
                return Resources.Nebula.ResourceManager.GetString(key) ?? key;
            return string.Format(Resources.Nebula.ResourceManager.GetString(key) ?? $"UNKNOWN_KEY({key})", format);
        }

        private static async void AppTick(CancellationToken token, int delay)
        {
            try
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    Discord.RunCallbacks();
                    Invoke(() => Tick?.Invoke(Application.Current, new EventArgs()));
                    await Task.Delay(delay, token);
                }
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        ///     Invoke on main thread
        /// </summary>
        /// <param name="action">Action to perform</param>
        public static void Invoke(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        /// <summary>
        ///     Invoke on main thread
        /// </summary>
        /// <param name="action">Action to perform</param>
        public static void BeginInvoke(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(action);
        }
    }
}