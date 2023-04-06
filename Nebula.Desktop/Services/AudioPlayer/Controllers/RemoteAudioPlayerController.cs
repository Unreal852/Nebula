using LiteNetLib;
using Nebula.Common.Audio;
using Nebula.Common.Medias;
using Nebula.Net;
using Nebula.Net.Packets.Requests;
using Nebula.Net.Packets.Responses;
using Nebula.Net.Services.Client;
using Nebula.Services.Contracts;
using Serilog;
using System;

namespace Nebula.Desktop.Services.AudioPlayer.Controllers;

public sealed class RemoteAudioPlayerController : IAudioPlayerController
{
    private readonly ILogger _logger;
    private readonly IAudioPlayerService _audioPlayerService;
    private readonly INetClientService _netClientService;
    private bool _isInitialized;

    public RemoteAudioPlayerController(ILogger logger, IAudioPlayerService audioPlayerService, INetClientService netClientService)
    {
        _logger = logger.ForContext("ClassContext", nameof(RemoteAudioPlayerController));
        _audioPlayerService = audioPlayerService;
        _netClientService = netClientService;

        _netClientService.SubscribePacket<YoutubeMusicResponsePacket>(OnReceiveYoutubeMusicResponse);
        _netClientService.SubscribePacket<PlayerPlayResponsePacket>(OnReceivePlayerPlayResponse);
        _netClientService.SubscribePacket<PlayerPauseResponsePacket>(OnReceivePlayerPauseResponse);
        _netClientService.SubscribePacket<PlayerPositionResponsePacket>(OnReceivePlayerPositionResponse);
        _netClientService.Disconnected += OnNetClientServiceDisconnected;
    }

    public event EventHandler<NetPeer> Connected
    {
        add => _netClientService.Connected += value;
        remove => _netClientService.Connected -= value;
    }

    public event EventHandler<NetPeer> Disconnected
    {
        add => _netClientService.Disconnected += value;
        remove => _netClientService.Disconnected -= value;
    }

    public void Connect(NetOptions netOptions, string username)
    {
        _netClientService.Connect(netOptions, username);
    }

    public void Initialize(params object[] args)
    {
        if (args.Length == 2 && args[0] is NetOptions options && args[1] is string username)
        {
            Connect(options, username);
        }
        else
            _logger.Warning("Failed to initialize ! Unsupported params");
    }

    public bool OpenMedia(IMediaInfo media)
    {
        if (!media.HasValidId())
        {
            _logger.Warning("Received media will invalid id");
            return false;
        }

        var requestPacket = new YoutubeMusicRequestPacket { VideoId = media.Id! };
        _netClientService.SendPacket(ref requestPacket);
        return false;
    }

    public bool OpenPlaylist(string playlistUrl)
    {
        return false;
    }

    public bool Play()
    {
        if (_audioPlayerService.IsPlaying)
            return false;
        var requestPacket = new PlayerPlayRequestPacket();
        _netClientService.SendPacket(ref requestPacket);
        return false;
    }

    public bool Pause()
    {
        if (_audioPlayerService.IsPaused)
            return false;
        var requestPacket = new PlayerPauseRequestPacket();
        _netClientService.SendPacket(ref requestPacket);
        return false;
    }

    public bool Skip()
    {
        return true;
    }

    public bool Stop()
    {
        return false;
    }

    public bool SetPosition(in TimeSpan position)
    {
        if (!_audioPlayerService.IsPlaying)
            return false;
        var requestPacket = new PlayerPositionRequestPacket { Position = position.TotalSeconds };
        _netClientService.SendPacket(ref requestPacket);
        return false;
    }

    public void OnRemoved()
    {
        //NetClientService.Stop();
        //NetClientService.UnsubscribePacketHandler<PlayerPlayResponsePacket>();
        //NetClientService.UnsubscribePacketHandler<PlayerPauseResponsePacket>();
        //NetClientService.UnsubscribePacketHandler<YoutubeMusicResponsePacket>();
        //NetClientService.Disconnected -= OnNetClientServiceDisconnected;
    }

    private void OnNetClientServiceDisconnected(object? sender, NetPeer? peer)
    {
        // Reset controller if we are disconnected
        _audioPlayerService.UpdateController(AudioPlayerControllerType.Local);
    }

    private async void OnReceiveYoutubeMusicResponse(YoutubeMusicResponsePacket packet)
    {
        await _audioPlayerService.OpenMedia(MediaInfo.FromId(packet.VideoId), true);
        var readyPacket = new ClientReadyRequestPacket();
        _netClientService.SendPacket(ref readyPacket);
    }

    private void OnReceivePlayerPlayResponse(PlayerPlayResponsePacket packet)
    {
        _audioPlayerService.Play(true);
    }

    private void OnReceivePlayerPauseResponse(PlayerPauseResponsePacket packet)
    {
        _audioPlayerService.Pause(true);
    }

    private void OnReceivePlayerPositionResponse(PlayerPositionResponsePacket packet)
    {
        TimeSpan position = TimeSpan.FromSeconds(packet.Position);
        _audioPlayerService.SetPosition(in position, true);
    }
}