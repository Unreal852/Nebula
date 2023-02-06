using CommunityToolkit.Mvvm.Messaging.Messages;
using Nebula.Common.Playlist;

namespace Nebula.Desktop.ViewModel.Messages;

public sealed class PlaylistChangeMessage : ValueChangedMessage<Playlist>
{
    public PlaylistChangeMessage(Playlist value) : base(value)
    {
    }
}