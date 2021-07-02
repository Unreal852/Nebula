using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using LiteMVVM;
using LiteMVVM.Command;
using Nebula.Core.Online;
using Nebula.Core.Online.Events;
using Nebula.Model;
using Nebula.Net.Client.Events;
using Nebula.Net.Data;
using Nebula.Net.Packet;
using Nebula.View;
using Nebula.View.Views;

namespace Nebula.ViewModel
{
    public class OnlineSessionViewModel : BaseViewModel
    {
        private string _currentMessage;

        public OnlineSessionViewModel()
        {
            SendMessageCommand = new RelayCommand(SendMessage);
            ClearMessagesCommand = new RelayCommand(ClearMessages);
            CopyAddressCommand = new RelayCommand(CopyAddress);
            DisconnectCommand = new RelayCommand(Disconnect);
            Session.SessionInfoChanged += OnSessionInfoChanged;
            Session.NewMessage += OnNewMessage;
            Session.UserConnected += OnUserConnected;
            Session.UserDisconnected += OnUserDisconnected;
            Session.Client.Connected += OnConnected;
            Session.Client.Disconnected += OnDisconnected;
        }

        public ICommand                          SendMessageCommand   { get; }
        public ICommand                          ClearMessagesCommand { get; }
        public ICommand                          CopyAddressCommand   { get; }
        public ICommand                          DisconnectCommand    { get; }
        public ObservableCollection<UserMessage> Messages             { get; } = new();
        public ObservableCollection<NetUserInfo> Users                { get; } = new();
        public OnlineSessionManager              Session              => NebulaClient.OnlineSession;
        public bool                              IsConnected          => Session.IsClientConnected;

        public string CurrentMessage
        {
            get => _currentMessage;
            set => Set(ref _currentMessage, value);
        }

        private void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(CurrentMessage))
                return;
            Session.SendClientPacket(new UserMessagePacket {Message = new NetMessage {Message = CurrentMessage, MessageType = 0}});
            CurrentMessage = string.Empty;
        }

        private void ClearMessages()
        {
            Messages.Clear();
        }

        private void Disconnect()
        {
            NebulaClient.OnlineSession.HostClient.StopAndDisconnect();
            OnDisconnected(null, null);
        }

        private void CopyAddress()
        {
            Clipboard.SetText(NebulaClient.OnlineSession.IpEndPoint.ToString());
        }

        private void OnSessionInfoChanged(object sender, SessionInfoChangedEventArgs e)
        {
            NebulaClient.Invoke(() =>
            {
                Users.Clear();
                if (e.Users == null)
                    return;
                foreach (NetUserInfo user in e.Users)
                {
                    if (string.IsNullOrWhiteSpace(user.Username))
                        continue;
                    Users.Add(user);
                }
            });
        }

        private void OnNewMessage(object sender, NewUserMessageEventArgs e)
        {
            NebulaClient.Invoke(() => Messages.Add(new UserMessage(e.User, e.Message)));
        }

        private void OnUserConnected(object sender, UserConnectedEventArgs e)
        {
            NebulaClient.Invoke(() =>
            {
                Users.Add(e.UserInfo);
                Messages.Add(new UserMessage(e.UserInfo, new NetMessage {MessageType = 2}));
            });
        }

        private void OnUserDisconnected(object sender, UserDisconnectedEventArgs e)
        {
            NebulaClient.Invoke(() =>
            {
                for (int i = Users.Count - 1; i >= 0; i--)
                {
                    if (Users[i].Id == e.UserInfo.Id)
                        Users.RemoveAt(i);
                }
                Messages.Add(new UserMessage(e.UserInfo, new NetMessage {MessageType = 3}));
            });
        }

        private void OnConnected(object sender, ConnectedEventArgs e)
        {
            Messages.Clear();
            Users.Clear();
        }

        private void OnDisconnected(object sender, DisconnectedEventArgs e)
        {
            if (ViewModelsLocator.Instance.MainWindowViewModel.IsCurrentPage(typeof(OnlineSessionView)))
                NebulaClient.Invoke(() => Messenger.Broadcast(this, NavigationInfo.Create(typeof(HomeView), null, true)));
        }
    }
}