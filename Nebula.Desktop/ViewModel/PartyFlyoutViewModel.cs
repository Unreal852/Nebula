using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiteNetLib;
using Nebula.Common.Audio;
using Nebula.Desktop.Services.AudioPlayer.Controllers;
using Nebula.Net;
using Nebula.Net.Extensions;
using Nebula.Net.Services;
using Nebula.Net.Services.Client;
using Nebula.Services.Contracts;
using Serilog;
using System.Threading.Tasks;

namespace Nebula.Desktop.ViewModel;

public sealed partial class PartyFlyoutViewModel : ViewModelBase
{
    private readonly IAudioPlayerService _audioPlayerService;
    private readonly INetServerService _netServerService;
    private readonly ISettingsService _settingsService;
    private readonly ILogger _logger;

    [ObservableProperty]
    private bool _isClientConnected;

    [ObservableProperty]
    private bool _isServerHost;

    public PartyFlyoutViewModel(ILogger logger, ISettingsService settingsService, IAudioPlayerService audioPlayerService, INetServerService netServerService)
    {
        _logger = logger;
        _settingsService = settingsService;
        _audioPlayerService = audioPlayerService;
        _netServerService = netServerService;
    }

    [RelayCommand]
    private void JoinSession()
    {
        var remoteController = _audioPlayerService.UpdateController(AudioPlayerControllerType.Remote);
        if (remoteController is RemoteAudioPlayerController remote)
        {
            remote.Connected += OnClientServiceConnected;
            remote.Disconnected += OnClientServiceDisconnected;
            remoteController.Initialize(_settingsService.Settings.GetNetOptions(), _settingsService.Settings.PartyUsername);
        }
    }

    [RelayCommand]
    private async Task HostSession()
    {
        await _netServerService.Start(_settingsService.Settings.GetNetOptions());
        IsServerHost = true;
        JoinSession();
    }

    private void OnClientServiceConnected(object? sender, NetPeer e)
    {
        IsClientConnected = true;
    }

    private void OnClientServiceDisconnected(object? sender, NetPeer e)
    {
        if (sender is NetClientService client)
        {
            client.Connected -= OnClientServiceConnected;
            client.Disconnected -= OnClientServiceDisconnected;
        }

        IsClientConnected = false;
    }
}
