using LiteNetLib;
using Nebula.Core.Providers.Youtube;
using Nebula.Model;
using Nebula.Net.Client;
using Nebula.Net.Client.Events;
using Nebula.Net.Data;
using Nebula.Net.Packet;
using Nebula.Net.Packet.C2S;
using Nebula.Net.Packet.S2C;
using Nebula.Net.Server.Events;

namespace Nebula.Core
{
    public class OnlineSessionManager
    {
        public OnlineSessionManager()
        {
            HostClient = new NetHostClient();
            HostClient.Server.ClientConnected += OnServerClientConnected;
            HostClient.Server.ClientDisconnected += OnServerClientDisconnected;
            HostClient.Client.Connected += OnClientConnected;
            HostClient.Client.Disconnected += OnClientDisconnected;
            HostClient.Client.NetProcessor.SubscribeReusable<SessionInfoPacket>(OnReceiveSessionInfoPacket);
            HostClient.Client.NetProcessor.SubscribeReusable<PlayerOpenMediaPacket>(OnReceivePlayerOpenMediaPacket);
            HostClient.Client.NetProcessor.SubscribeReusable<PlayerPausePacket>(OnReceivePlayerPausePacket);
            HostClient.Client.NetProcessor.SubscribeReusable<PlayerResumePacket>(OnReceivePlayerResumePacket);
            HostClient.Client.NetProcessor.SubscribeReusable<PlayerPlayPacket>(OnReceivePlayerPlayPacket);
            HostClient.Client.NetProcessor.SubscribeReusable<PlayerPositionPacket>(OnReceivePlayerPositionPacket);
        }

        public NetHostClient  HostClient        { get; }
        public NetSessionInfo SessionInfo       { get; private set; } = NetSessionInfo.Default;
        public bool           IsClientConnected => HostClient.Client.IsConnected;
        public bool           IsServerRunning   => HostClient.Server.IsRunning;

        public void SendClientPacket<T>(T packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            HostClient.Client.SendPacket(packet, method);
        }

        private void OnServerClientConnected(object sender, ClientConnectedEventArgs e)
        {
        }

        private void OnServerClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
        }

        private void OnClientConnected(object sender, ConnectedEventArgs e)
        {
            SendClientPacket(new UserInfoPacket {UserInfo = new NetUserInfo {Username = "Unreal", AvatarUrl = ""}});
            NebulaClient.Discord.UpdateActivity();
        }

        private void OnClientDisconnected(object sender, DisconnectedEventArgs e)
        {
            SessionInfo = NetSessionInfo.Default;
        }

        private void OnReceiveSessionInfoPacket(SessionInfoPacket packet)
        {
            SessionInfo = packet.SessionInfo;
        }

        private void OnReceivePlayerPausePacket(PlayerPausePacket packet)
        {
            NebulaClient.MediaPlayer.Pause(true);
        }

        private void OnReceivePlayerResumePacket(PlayerResumePacket packet)
        {
            NebulaClient.MediaPlayer.Resume(true);
        }

        private void OnReceivePlayerPlayPacket(PlayerPlayPacket packet)
        {
            NebulaClient.MediaPlayer.Play(true);
        }

        private void OnReceivePlayerPositionPacket(PlayerPositionPacket packet)
        {
            NebulaClient.MediaPlayer.SetPosition(packet.Position, true);
        }

        private async void OnReceivePlayerOpenMediaPacket(PlayerOpenMediaPacket packet)
        {
            MediaInfo mediaInfo = await NebulaClient.Providers.FindProviderByType<YoutubeMediaProvider>().GetMediaInfo(packet.Media.Id);
            if (mediaInfo == null)
                return;
            await NebulaClient.MediaPlayer.OpenMedia(mediaInfo, fromRemote: true);
            SendClientPacket(new PlayerReadyPacket());
        }
    }
}