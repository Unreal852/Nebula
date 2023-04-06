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
    private readonly IAudioPlayerService _audioPlayerService;
    public readonly INetClientService NetClientService;
    private bool _isInitialized;

    public RemoteAudioPlayerController(IAudioPlayerService audioPlayerService, INetClientService netClientService)
    {
        _audioPlayerService = audioPlayerService;
        NetClientService = netClientService;
    }

    public void Initialize(params object[] args)
    {
        if (_isInitialized)
            return;
        if (args.Length != 1 || args[0] is not NetOptions options)
            return;
        _isInitialized = true;
        NetClientService.SubscribePacket<YoutubeMusicResponsePacket>(OnReceiveYoutubeMusicResponse);
        NetClientService.SubscribePacket<PlayerPlayResponsePacket>(OnReceivePlayerPlayResponse);
        NetClientService.SubscribePacket<PlayerPauseResponsePacket>(OnReceivePlayerPauseResponse);
        NetClientService.SubscribePacket<PlayerPositionResponsePacket>(OnReceivePlayerPositionResponse);
        NetClientService.Disconnected += OnNetClientServiceDisconnected;
        //NetClientService.NetOptions = options;
        //NetClientService.Start();
    }

    public bool OpenMedia(IMediaInfo media)
    {
        if (!media.HasValidId())
        {
            Log.Warning("[RemoteAudioPlayerController] Received media will invalid id");
            return false;
        }

        var requestPacket = new YoutubeMusicRequestPacket { VideoId = media.Id! };
        NetClientService.SendPacket(ref requestPacket);
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
        NetClientService.SendPacket(ref requestPacket);
        return false;
    }

    public bool Pause()
    {
        if (_audioPlayerService.IsPaused)
            return false;
        var requestPacket = new PlayerPauseRequestPacket();
        NetClientService.SendPacket(ref requestPacket);
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
        NetClientService.SendPacket(ref requestPacket);
        return false;
    }

    public void OnRemoved()
    {
        //NetClientService.Stop();
        //NetClientService.UnsubscribePacketHandler<PlayerPlayResponsePacket>();
        //NetClientService.UnsubscribePacketHandler<PlayerPauseResponsePacket>();
        //NetClientService.UnsubscribePacketHandler<YoutubeMusicResponsePacket>();
        NetClientService.Disconnected -= OnNetClientServiceDisconnected;
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
        NetClientService.SendPacket(ref readyPacket);
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