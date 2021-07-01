using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using EasyLogger;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Nebula.Core;
using Nebula.Core.Database;
using Nebula.Core.Logs;
using Nebula.Core.Online;
using Nebula.Core.Player;
using Nebula.Core.Playlists;
using Nebula.Core.Providers;
using Nebula.Core.Settings;
using Nebula.Core.Update;
using SharpToolbox.Windows.Hookers;
using SharpToolbox.Windows.Hookers.Keyboard;

namespace Nebula
{
    public static class NebulaClient
    {
        static NebulaClient()
        {
            Settings = AppSettings.LoadSettings();
            if (Settings.Privacy.AllowAnalytics || Settings.Privacy.AllowCrashReports)
            {
                AppCenter.Start("df3a859e-110a-43b2-892d-71f4650c9c70",
                    Settings.Privacy.AllowAnalytics ? typeof(Analytics) : null,
                    Settings.Privacy.AllowCrashReports ? typeof(Crashes) : null);
            }

            Logger = new Logger();
#if DEBUG
            Logger.HandlersManager.AddHandler(new DebugLogHandler());
#endif
            Playlists = new PlaylistsManager();
            Providers = new ProvidersManager();
            Database = new NebulaDatabase();
            OnlineSession = new OnlineSessionManager();
            MediaPlayer = new NAudioPlayer();
            Discord = new DiscordManager();
            UpdateManager = new UpdateManager();
            CancellationTokenSource = new CancellationTokenSource();
            KeyboardHook = new KeyboardHook();

            Discord.UpdateActivity();
            UpdateManager.CheckForUpdateNotify();
            KeyboardHook.KeyDown += OnGlobalKeyDown;
            if (Settings.General.KeyboardMediaKeysSupport)
                KeyboardHook.Hook();
            Task.Run(() => AppTick(CancellationTokenSource.Token, 500));
        }

        public static   AppSettings             Settings                { get; }
        public static   ProvidersManager        Providers               { get; }
        public static   NAudioPlayer            MediaPlayer             { get; }
        public static   NebulaDatabase          Database                { get; }
        public static   PlaylistsManager        Playlists               { get; }
        public static   DiscordManager          Discord                 { get; }
        public static   OnlineSessionManager    OnlineSession           { get; }
        public static   UpdateManager           UpdateManager           { get; }
        public static   KeyboardHook            KeyboardHook            { get; }
        public static   Logger                  Logger                  { get; }
        internal static CancellationTokenSource CancellationTokenSource { get; }

        public static event EventHandler Tick;

        public static string GetLang(string key, params object[] format)
        {
            if (format is not {Length: > 0})
                return Resources.Nebula.ResourceManager.GetString(key) ?? key;
            return string.Format(Resources.Nebula.ResourceManager.GetString(key) ?? $"UNKNOWN_KEY({key})", format);
        }

        private static void OnGlobalKeyDown(object sender, KeyboardKeyDownEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKeys.MEDIA_PLAY_PAUSE:
                    if (MediaPlayer.IsPaused)
                        MediaPlayer.Resume();
                    else
                        MediaPlayer.Pause();
                    e.Handled = true;
                    break;
                case VirtualKeys.MEDIA_STOP:
                    MediaPlayer.Stop(true);
                    e.Handled = true;
                    break;
                case VirtualKeys.MEDIA_NEXT_TRACK:
                    MediaPlayer.Forward(true);
                    e.Handled = true;
                    break;
                case VirtualKeys.VOLUME_MUTE:
                    MediaPlayer.IsMuted = !MediaPlayer.IsMuted;
                    e.Handled = true;
                    break;
                case VirtualKeys.VOLUME_UP:
                    MediaPlayer.Volume += Settings.General.KeyboardMediaKeysSoundIncDevValue;
                    e.Handled = true;
                    break;
                case VirtualKeys.VOLUME_DOWN:
                    MediaPlayer.Volume -= Settings.General.KeyboardMediaKeysSoundIncDevValue;
                    e.Handled = true;
                    break;
            }
        }

        private static async void AppTick(CancellationToken token, int delay)
        {
            try
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
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

        public static async void Restart(bool ask = true)
        {
            if (ask)
            {
                Dialog dialog = NebulaDialog.ShowWarningNoYes("dialog_restart", "dialog_title_restart");
                var result = await dialog.GetResultAsync<bool>();
                if (!result)
                    return;
            }

            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Application.Current.Shutdown();
        }
    }
}