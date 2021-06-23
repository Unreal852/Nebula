using System.Collections.ObjectModel;
using System.Windows.Input;
using LiteMVVM;
using LiteMVVM.Command;
using Nebula.Core.Online;
using Nebula.Core.Online.Events;
using Nebula.Model;
using Nebula.Net.Client.Events;
using Nebula.Net.Data;
using Nebula.Net.Packet;

namespace Nebula.ViewModel
{
    public class OnlineSessionViewModel : BaseViewModel
    {
        private string _currentMessage;

        public OnlineSessionViewModel()
        {
            SendMessageCommand = new RelayCommand(SendMessage);
            ClearMessagesCommand = new RelayCommand(ClearMessages);
            Session.SessionInfoChanged += OnSessionInfoChanged;
            Session.NewMessage += OnNewMessage;
            Session.UserConnected += OnUserConnected;
            Session.UserDisconnected += OnUserDisconnected;
            Session.Client.Connected += OnConnected;
            Session.Client.Disconnected += OnDisconnected;
        }

        public ICommand                          SendMessageCommand   { get; }
        public ICommand                          ClearMessagesCommand { get; }
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

        private void OnSessionInfoChanged(object sender, SessionInfoChangedEventArgs e)
        {
            NebulaClient.Invoke(() =>
            {
                Users.Clear();
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
            NebulaClient.Invoke(() => Users.Add(e.UserInfo));
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
            });
        }

        private void OnConnected(object sender, ConnectedEventArgs e)
        {
            Messages.Clear();
            Users.Clear();
        }

        private void OnDisconnected(object sender, DisconnectedEventArgs e)
        {
        }
    }
}