using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nebula.Net.Services;
using Nebula.Services.Contracts;
using Serilog;

namespace Nebula.Desktop.ViewModel;

public sealed partial class PartyFlyoutViewModel : ViewModelBase
{
    private readonly IAudioPlayerService _audioPlayerService;
    private readonly INetServerService _netServerService;
    private readonly ILogger _logger;

    [ObservableProperty]
    private bool _isClientConnected;

    [ObservableProperty]
    private bool _isServerHost;

    public PartyFlyoutViewModel(ILogger logger, IAudioPlayerService audioPlayerService, INetServerService netServerService)
    {
        _logger = logger;
        _audioPlayerService = audioPlayerService;
        _netServerService = netServerService;
    }

    [RelayCommand]
    private void JoinSession()
    {
        _logger.Information("Join session");
    }

    [RelayCommand]
    private void HostSession()
    {
        _logger.Information("Host session");

    }
}
