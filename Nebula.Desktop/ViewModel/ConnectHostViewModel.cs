using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using Nebula.Net;

namespace Nebula.Desktop.ViewModel;

public sealed partial class ConnectHostViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _connectionString = string.Empty;

    [ObservableProperty]
    private string _serverAddress = "127.0.0.1";

    [ObservableProperty]
    private string _serverPassword = "";

    [ObservableProperty]
    private int _serverPort = 22855;

    [ObservableProperty]
    private int _serverSlots = 5;

    [ObservableProperty]
    private bool _useUpnp = true;

    public ConnectHostViewModel()
    {
#if DEBUG
        _connectionString
                = $"{ServerAddress}:{ServerPort}{(string.IsNullOrWhiteSpace(_serverPassword) ? "" : $":{_connectionString}")}";
#endif
    }

    partial void OnConnectionStringChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        string[] split = value.Split(':');
        if (split.Length < 2)
            return;
        if (!string.IsNullOrWhiteSpace(split[0]) && IPAddress.TryParse(split[0], out _))
            ServerAddress = split[0];
        if (!string.IsNullOrWhiteSpace(split[1]) && int.TryParse(split[1], out int result))
            ServerPort = result;
        if (split.Length > 2 && !string.IsNullOrWhiteSpace(split[2]))
            ServerPassword = split[2];
    }

    public NetOptions GetNetOptions()
    {
        return new NetOptions(ServerAddress, ServerPort, ServerSlots, ServerPassword, UseUpnp);
    }
}