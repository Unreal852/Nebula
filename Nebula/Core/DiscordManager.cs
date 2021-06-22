using System;
using HandyControl.Controls;
using Nebula.Core.Extensions;
using Nebula.Discord.SDK;
using Nebula.Model;

namespace Nebula.Core
{
    public class DiscordManager
    {
        public DiscordManager()
        {
            NebulaClient.Tick += (_, _) => Discord.RunCallbacks();
            Discord.GetActivityManager().OnActivityJoin += OnActivityJoin;
        }

        private Discord.SDK.Discord Discord { get; } = new(740292732794306690, (ulong) CreateFlags.Default);

        public void ClearActivity()
        {
            Discord.GetActivityManager().ClearActivity(_ => { });
        }

        public bool SetActivity(MediaInfo mediaInfo, TimeSpan currentPosition = default)
        {
            Activity activity = new Activity
            {
                Name = mediaInfo.Title,
                State = "Listening",
                Details = $"{mediaInfo.Title} - {mediaInfo.Author}",
                Type = ActivityType.Listening,
                Instance = true,
                Timestamps =
                {
                    Start = (long) currentPosition.ToUnixMilliseconds(),
                    End = (long) (mediaInfo.Duration - currentPosition).ToUnixMilliseconds()
                },
                Assets = {LargeImage = "nebula_icon", LargeText = $"Listening {mediaInfo.Title}"},
                Party = {Id = "dummy", Size = {CurrentSize = 1, MaxSize = 4}},
                Secrets = {Join = "127.0.0.1"}
            };
            bool success = false;
            Discord.GetActivityManager().UpdateActivity(activity, result => { success = result == Result.Ok; });
            return success;
        }

        private void OnActivityJoin(string secret)
        {
            NebulaClient.Invoke(() => Growl.Info("DISCORD JOIN : " + secret));
        }
    }
}