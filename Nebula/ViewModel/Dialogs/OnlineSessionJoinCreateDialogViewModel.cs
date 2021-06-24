using System.Net;
using Nebula.Net.Server;

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
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && value.Contains(":"))
                {
                    string[] split = value.Split(':');
                    if (split.Length == 2)
                    {
                        Set(ref _ipAddress, split[0]);
                        Port = int.Parse(split[1]);
                    }
                }
                else
                    Set(ref _ipAddress, value);
            }
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
                if (!IPAddress.TryParse(IpAddress, out _))
                    return;
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

            Close();
        }
    }
}