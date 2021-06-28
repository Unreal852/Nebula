using System.Net;
using Nebula.Core.Extensions;
using Nebula.Core.Player.Events;
using Nebula.Discord.SDK;
using Nebula.Model;
using Nebula.Net.Data;

namespace Nebula.Core
{
    public class DiscordManager
    {
        public DiscordManager()
        {
            try
            {
                Discord = new Discord.SDK.Discord(740292732794306690, (ulong) CreateFlags.NoRequireDiscord);
                Discord.GetActivityManager().OnActivityJoin += OnActivityJoin;
                NebulaClient.Tick += (_, _) => Discord.RunCallbacks();
                NebulaClient.MediaPlayer.MediaChanged += OnMediaPlayerOnMediaChanged;
            }
            catch
            {
                // ignored, this commonly means that discord is not running
            }
        }

        private Discord.SDK.Discord Discord  { get; }
        private bool                IsActive => Discord != null;

        public void ClearActivity()
        {
            if (!IsActive)
                return;
            Discord.GetActivityManager().ClearActivity(_ => { });
        }

        public bool UpdateActivity()
        {
            if (!IsActive || !NebulaClient.Settings.General.AllowDiscordIntegration)
                return false;
            (string Name, string State, string Details) activityInfo = GetActivityInfos();
            ActivityParty party = GetActivityParty();
            ActivitySecrets secrets = GetActivityPartySecrets();
            ActivityTimestamps timestamps = GetActivityTimeStamps();
            var activity = new Activity
            {
                Name = activityInfo.Name,
                State = activityInfo.State,
                Details = activityInfo.Details,
                Type = ActivityType.Listening,
                Instance = true,
                Timestamps = timestamps,
                Party = party,
                Secrets = secrets,
                Assets = {LargeImage = "nebula_icon", LargeText = $"Listening {activityInfo.Details}"}
            };
            var success = false;
            Discord.GetActivityManager().UpdateActivity(activity, result => { success = result == Result.Ok; });
            return success;
        }

        private (string Name, string State, string Details) GetActivityInfos()
        {
            MediaInfo mediaInfo = NebulaClient.MediaPlayer.CurrentMedia;
            var name = "";
            string state = mediaInfo == null ? "Idle" : "Listening";
            string details = mediaInfo == null ? "" : $"{mediaInfo.Title} - {mediaInfo.AuthorName}";
            return (name, state, details);
        }

        private ActivityTimestamps GetActivityTimeStamps()
        {
            MediaInfo mediaInfo = NebulaClient.MediaPlayer.CurrentMedia;
            if (mediaInfo == null)
                return default;
            return new ActivityTimestamps
            {
                Start = (long) NebulaClient.MediaPlayer.Position.ToUnixMilliseconds(),
                End = (long) (mediaInfo.Duration - NebulaClient.MediaPlayer.Position).ToUnixMilliseconds()
            };
        }

        private ActivityParty GetActivityParty()
        {
            NetSessionInfo sessionInfo = NebulaClient.OnlineSession.SessionInfo;
            if (sessionInfo.Id == -1)
                return default;
            return new ActivityParty {Id = sessionInfo.Id.ToString(), Size = {CurrentSize = sessionInfo.ClientsCount, MaxSize = sessionInfo.MaxClients}};
        }

        private ActivitySecrets GetActivityPartySecrets()
        {
            NetSessionInfo sessionInfo = NebulaClient.OnlineSession.SessionInfo;
            if (sessionInfo.Id == -1)
                return default;
            IPEndPoint endPoint = NebulaClient.OnlineSession.IpEndPoint;
            return new ActivitySecrets {Join = $"{endPoint.Address}:{endPoint.Port}"};
        }

        private void OnActivityJoin(string secret)
        {
            string[] ipPort = secret.Split(':');
            if (ipPort.Length <= 1)
                return;
            string ip = ipPort[0];
            int port = int.Parse(ipPort[1]);
            NebulaClient.OnlineSession.HostClient.Client.Connect(ip, port);
        }

        private void OnMediaPlayerOnMediaChanged(object sender, PlayerMediaChangedEventArgs e)
        {
            UpdateActivity();
        }
    }
}