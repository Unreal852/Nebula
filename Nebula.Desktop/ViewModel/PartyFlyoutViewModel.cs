using CommunityToolkit.Mvvm.ComponentModel;
using Nebula.Net.Services.Server;
using Nebula.Services.Contracts;

namespace Nebula.Desktop.ViewModel;

public sealed partial class PartyFlyoutViewModel : ViewModelBase
{
    private readonly IAudioPlayerService _audioPlayerService;
    private readonly INetServerService _nebulaServerService;

    [ObservableProperty]
    private bool _isClientConnected;

    [ObservableProperty]
    private bool _isServerHost;

    public PartyFlyoutViewModel(IAudioPlayerService audioPlayerService, INetServerService nebulaServerService)
    {
        _audioPlayerService = audioPlayerService;
        _nebulaServerService = nebulaServerService;
    }
}
