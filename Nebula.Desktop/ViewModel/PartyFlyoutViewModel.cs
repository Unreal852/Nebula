using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiteNetLib;
using Nebula.Common.Audio;
using Nebula.Desktop.Services.AudioPlayer.Controllers;
using Nebula.Net;
using Nebula.Net.Extensions;
using Nebula.Net.Packets.Responses;
using Nebula.Net.Services;
using Nebula.Net.Services.Client;
using Nebula.Services.Contracts;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nebula.Desktop.ViewModel;

public sealed partial class PartyFlyoutViewModel : ViewModelBase
{
    private const string IpInfoUrl = "https://ipinfo.io/ip";
    private readonly HttpClient _httpClient = new(new SocketsHttpHandler()
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(1)
    });
    private readonly IAudioPlayerService _audioPlayerService;
    private readonly INetServerService _netServerService;
    private readonly INetClientService _netClientService;
    private readonly ISettingsService _settingsService;
    private readonly ILogger _logger;

    [ObservableProperty]
    private bool _isClientConnected;

    [ObservableProperty]
    private bool _isServerHost;

    public PartyFlyoutViewModel(ILogger logger, ISettingsService settingsService, IAudioPlayerService audioPlayerService,
        INetServerService netServerService,
        INetClientService netClientService)
    {
        _logger = logger;
        _settingsService = settingsService;
        _audioPlayerService = audioPlayerService;
        _netServerService = netServerService;
        _netClientService = netClientService;

        _netClientService.Connected += OnClientServiceConnected;
        _netClientService.Disconnected += OnClientServiceDisconnected;
        _netClientService.SubscribePacket<ClientsListPacket>(OnReceiveClientsListPacket);
    }

    public ObservableCollection<ClientInfo> RemoteClients { get; set; } = new();


    [RelayCommand]
    private void JoinSession()
    {
        var remoteController = _audioPlayerService.UpdateController(AudioPlayerControllerType.Remote);
        if (remoteController is RemoteAudioPlayerController)
        {
            remoteController.Initialize(_settingsService.Settings.GetNetOptions(), _settingsService.Settings.PartyUsername);
        }
    }

    [RelayCommand]
    private async Task HostSession()
    {
        IsServerHost = true;
        await _netServerService.Start(_settingsService.Settings.GetNetOptions());
        JoinSession();
    }

    [RelayCommand]
    private async Task Disconnect()
    {
        if (IsClientConnected)
        {
            _netClientService.Disconnect();
        }

        if (IsServerHost)
        {
            await _netServerService.Stop();
            IsServerHost = false;
        }
    }

    [RelayCommand]
    private async Task CopyPublicIpAddress()
    {
        var result = await _httpClient.GetStringAsync(IpInfoUrl);
        if (!string.IsNullOrEmpty(result) && Application.Current is { Clipboard: { } clipboard })
        {
            await clipboard.SetTextAsync(result);
        }
    }

    private void OnClientServiceConnected(object? sender, NetPeer e)
    {
        IsClientConnected = true;
    }

    private void OnClientServiceDisconnected(object? sender, NetPeer? e)
    {
        ClearRemoteClients();
        IsClientConnected = false;
    }

    private void OnReceiveClientsListPacket(ClientsListPacket packet)
    {
        if (packet.Clients == null)
        {
            _logger.Warning("Received empty {PacketType}", nameof(ClientsListPacket));
            return;
        }

        ClearRemoteClients();

        foreach (var client in packet.Clients)
        {
            RemoteClients.Add(client);
        }
    }

    private void ClearRemoteClients() // RemoteClients.Clear() does nothing ???, i use thing instead
    {
        while (RemoteClients.Count != 0)
        {
            RemoteClients.RemoveAt(0);
        }
    }
}
