using System;
using System.Net;
using HandyControl.Controls;
using LiteMVVM.Messenger;
using LiteNetLib;
using Nebula.Core.Online.Events;
using Nebula.Core.Providers.Youtube;
using Nebula.Model;
using Nebula.Net.Client;
using Nebula.Net.Client.Events;
using Nebula.Net.Data;
using Nebula.Net.Packet;
using Nebula.Net.Packet.C2S;
using Nebula.Net.Packet.S2C;
using Nebula.Net.Server;
using Nebula.View;
using Nebula.View.Views;

namespace Nebula.Core.Online
{
    public class OnlineSessionManager
    {
        public OnlineSessionManager()
        {
            HostClient = new NetHostClient();
            SetSessionInfo(NetSessionInfo.Default, null);
            HostClient.Client.Connected += OnClientConnected;
            HostClient.Client.Disconnected += OnClientDisconnected;
            HostClient.Client.NetProcessor.SubscribeReusable<SessionInfoPacket>(OnReceiveSessionInfoPacket);
            HostClient.Client.NetProcessor.SubscribeReusable<PlayerOpenMediaPacket>(OnReceivePlayerOpenMediaPacket);
            HostClient.Client.NetProcessor.SubscribeReusable<PlayerPausePacket>(OnReceivePlayerPausePacket);
            HostClient.Client.NetProcessor.SubscribeReusable<PlayerResumePacket>(OnReceivePlayerResumePacket);
            HostClient.Client.NetProcessor.SubscribeReusable<PlayerPlayPacket>(OnReceivePlayerPlayPacket);
            HostClient.Client.NetProcessor.SubscribeReusable<PlayerPositionPacket>(OnReceivePlayerPositionPacket);
            HostClient.Client.NetProcessor.SubscribeReusable<UserMessagePacket>(OnReceiveUserMessagePacket);
            HostClient.Client.NetProcessor.SubscribeReusable<UserConnectedPacket>(OnReceiveUserConnectedPacket);
            HostClient.Client.NetProcessor.SubscribeReusable<UserDisconnectedPacket>(OnReceiveUserDisconnectedPacket);
        }

        public NetHostClient  HostClient        { get; }
        public NetSessionInfo SessionInfo       { get; private set; }
        public NetClient      Client            => HostClient.Client;
        public NetServer      Server            => HostClient.Server;
        public bool           IsClientConnected => HostClient.Client.IsConnected;
        public bool           IsServerRunning   => HostClient.Server.IsRunning;

        public IPEndPoint IpEndPoint
        {
            get
            {
                if (IsServerRunning)
                    return new IPEndPoint(HostClient.IpAddress, Server.Settings.ServerPort);
                return new IPEndPoint(Client.ServerPeer.EndPoint.Address, Client.ServerPeer.EndPoint.Port);
            }
        }

        public event EventHandler<SessionInfoChangedEventArgs> SessionInfoChanged;
        public event EventHandler<NewUserMessageEventArgs>     NewMessage;
        public event EventHandler<UserConnectedEventArgs>      UserConnected;
        public event EventHandler<UserDisconnectedEventArgs>   UserDisconnected;

        public void SendClientPacket<T>(T packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            HostClient.Client.SendPacket(packet, method);
        }

        private void SetSessionInfo(NetSessionInfo sessionInfo, NetUserInfo[] users)
        {
            SessionInfo = sessionInfo;
            SessionInfoChanged?.Invoke(this, new SessionInfoChangedEventArgs(sessionInfo, users));
            NebulaClient.Discord?.UpdateActivity();
        }

        private void OnClientConnected(object sender, ConnectedEventArgs e)
        {
            SendClientPacket(new UserInfoPacket {UserInfo = new NetUserInfo {Username = "Unreal", AvatarUrl = ""}});
            NebulaClient.Invoke(() => Messenger.Default.Broadcast(this, NavigationInfo.Create(typeof(OnlineSessionView), null, true)));
        }

        private void OnClientDisconnected(object sender, DisconnectedEventArgs e)
        {
            SetSessionInfo(NetSessionInfo.Default, null);
            Growl.Warning($"Disconnected : {e.Info.Reason}");
        }

        private void OnReceiveSessionInfoPacket(SessionInfoPacket packet)
        {
            SetSessionInfo(packet.SessionInfo, packet.Users);
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

        private void OnReceiveUserMessagePacket(UserMessagePacket packet)
        {
            NewMessage?.Invoke(this, new NewUserMessageEventArgs(packet.Sender, packet.Message));
        }

        private void OnReceiveUserConnectedPacket(UserConnectedPacket packet)
        {
            UserConnected?.Invoke(this, new UserConnectedEventArgs(packet.User));
        }

        private void OnReceiveUserDisconnectedPacket(UserDisconnectedPacket packet)
        {
            UserDisconnected?.Invoke(this, new UserDisconnectedEventArgs(packet.User));
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