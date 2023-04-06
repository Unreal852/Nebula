using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using LiteNetLib;
using Nebula.Common.Audio;
using Nebula.Desktop.Properties;
using Nebula.Desktop.Services.AudioPlayer.Controllers;
using Nebula.Desktop.View.Dialogs;
using Nebula.Net;
using Nebula.Net.Services;
using Nebula.Net.Services.Client;
using Nebula.Services.Contracts;

namespace Nebula.Desktop.ViewModel;

public sealed partial class SharedSessionPageViewModel : ViewModelPageBase
{
    private readonly INetServerService _netServerService;
    private readonly IAudioPlayerService _audioPlayerService;

    [ObservableProperty]
    private bool _isClientConnected;

    [ObservableProperty]
    private bool _isServerHost;

    public SharedSessionPageViewModel(IAudioPlayerService audioPlayerService, INetServerService nebulaServerService)
    {
        PageName = Resources.PageSharedSession;
        PageIcon = Symbol.PeopleFilled;
        _audioPlayerService = audioPlayerService;
        _netServerService = nebulaServerService;
    }

    public override void OnNavigatedTo()
    {
        if (!IsClientConnected)
            ShowConnectHostDialog();
    }

    public async Task StartServerAndConnect(NetOptions netOptions)
    {
        //_netServerService.NetOptions = netOptions;
        //await _netServerService.Start();
        IsServerHost = true;
        ConnectToServer(netOptions);
    }

    public Task StopServer()
    {
        return IsServerHost ? _netServerService.Stop() : Task.CompletedTask;
    }

    public void ConnectToServer(NetOptions netOptions)
    {
        IAudioPlayerController? remoteController
                = _audioPlayerService.UpdateController(AudioPlayerControllerType.Remote);
        if (remoteController is RemoteAudioPlayerController remote)
        {
            //remote.NetClientService.Connecting += OnClientServiceConnecting;
            //remote.NetClientService.Connected += OnClientServiceConnected;
            //remote.NetClientService.Disconnected += OnClientServiceDisconnected;
            remoteController.Initialize(netOptions);
        }
    }

    [RelayCommand]
    public async void ShowConnectHostDialog()
    {
        if (_netServerService is { IsRunning: true })
            return;

        ContentDialog dialog
                = Dialog.CreateDialog<ConnectHostDialogView>("Shared session", "Connect", "Host", "Cancel");
        ContentDialogResult dialogResult = await dialog.ShowAsync();
        if (dialog.Content is UserControl { DataContext: ConnectHostViewModel viewModel })
            switch (dialogResult)
            {
                case ContentDialogResult.Primary:
                    ConnectToServer(viewModel.GetNetOptions());
                    break;
                case ContentDialogResult.Secondary:
                    await StartServerAndConnect(viewModel.GetNetOptions());
                    break;
            }
    }

    private void OnClientServiceConnecting(object? sender, EventArgs e)
    {
        //IsClientConnected = true;
    }

    private void OnClientServiceConnected(object? sender, NetPeer e)
    {
        IsClientConnected = true;
    }

    private void OnClientServiceDisconnected(object? sender, NetPeer e)
    {
        if (sender is NetClientService client)
        {
            client.Connecting -= OnClientServiceConnecting;
            client.Connected -= OnClientServiceConnected;
            client.Disconnected -= OnClientServiceDisconnected;
        }

        IsClientConnected = false;
    }
}