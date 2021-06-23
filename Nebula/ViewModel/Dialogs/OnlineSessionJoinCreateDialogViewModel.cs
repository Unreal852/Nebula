using Nebula.Net.Server;
using Nebula.View;
using Nebula.View.Views;

namespace Nebula.ViewModel.Dialogs
{
    public class OnlineSessionJoinCreateDialogViewModel : BaseDialogViewModel
    {
        private string _ipAddress;
        private string _key;
        private int    _port;
        private int    _size;
        private int    _tabIndex;
        private bool   _useUpnp;

        public OnlineSessionJoinCreateDialogViewModel()
        {
            Title = NebulaClient.GetLang("global_shared_session");
            Port = 9080;
            Size = 10;
            UseUpnp = true;
        }

        public string IpAddress
        {
            get => _ipAddress;
            set => Set(ref _ipAddress, value);
        }

        public string Key
        {
            get => _key;
            set => Set(ref _key, value);
        }

        public int Port
        {
            get => _port;
            set => Set(ref _port, value);
        }

        public int Size
        {
            get => _size;
            set => Set(ref _size, value);
        }

        public int TabIndex
        {
            get => _tabIndex;
            set => Set(ref _tabIndex, value);
        }

        public bool UseUpnp
        {
            get => _useUpnp;
            set => Set(ref _useUpnp, value);
        }

        protected override async void OnConfirm()
        {
            if (TabIndex == 0) // Join
            {
                NebulaClient.OnlineSession.HostClient.Connect(IpAddress, Port, Key);
            }
            else // Create
            {
                await NebulaClient.OnlineSession.HostClient.HostAndConnect(new NetServerSettings
                {
                    Key = Key,
                    MaxClients = Size,
                    ServerPort = Port,
                    UseUpnp = UseUpnp,
                    BadPacketsLimit = 100,
                    MediaChangeDelay = 2000,
                    UpNpTimeOut = 10000
                });
            }

            Messenger.Broadcast(this, NavigationInfo.Create(typeof(OnlineSessionView), null, false));
            Close();
        }
    }
}