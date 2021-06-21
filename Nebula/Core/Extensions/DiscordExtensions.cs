using System;
using Nebula.Core.Discord;
using Nebula.Model;

namespace Nebula.Core.Extensions
{
    public static class DiscordExtensions
    {
        public static bool SetActivity(this Discord.Discord discord, MediaInfo mediaInfo, TimeSpan currentPosition = default)
        {
            Activity activity = new Activity
            {
                State = "Listening",
                Details = $"{mediaInfo.Title} - {mediaInfo.Author}",
                Timestamps = new ActivityTimestamps
                    {Start = (long) currentPosition.ToUnixMilliseconds(), End = (long) (mediaInfo.Duration - currentPosition).ToUnixMilliseconds()},
                Assets = new ActivityAssets {LargeImage = "nebula_icon", LargeText = $"Listening {mediaInfo.Title}"},
                Instance = true,
            };
            bool success = false;
            discord.GetActivityManager().UpdateActivity(activity, result => { success = result == Result.Ok; });
            return success;
        }
    }
}