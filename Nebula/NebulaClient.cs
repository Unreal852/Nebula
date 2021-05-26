using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using HandyControl.Controls;
using Nebula.Core;
using Nebula.Core.Database;
using Nebula.Core.Player;
using Nebula.Core.Providers;
using Nebula.Media.Player;
using MessageBox = System.Windows.MessageBox;

namespace Nebula
{
    public static class NebulaClient
    {
        public static   ProvidersManager        Providers               { get; }
        public static   NAudioPlayer            MediaPlayer             { get; }
        public static   NebulaDatabase          Database                { get; }
        public static   PlaylistsManager        Playlists               { get; }
        internal static CancellationTokenSource CancellationTokenSource { get; }

        public static event EventHandler Tick;

        static NebulaClient()
        {
            Providers = new();
            Database = new NebulaDatabase();
            // Order does not matter below 
            Playlists = new PlaylistsManager();
            MediaPlayer = new NAudioPlayer();
            CancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => AppTick(CancellationTokenSource.Token, 500));
        }
        
        
        public static string GetLang(string key, params object[] format)
        {
            if (format == null || format.Length == 0)
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
        /// Invoke on main thread
        /// </summary>
        /// <param name="action">Action to perform</param>
        public static void Invoke(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        /// <summary>
        /// Invoke on main thread
        /// </summary>
        /// <param name="action">Action to perform</param>
        public static void BeginInvoke(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(action);
        }
    }
}