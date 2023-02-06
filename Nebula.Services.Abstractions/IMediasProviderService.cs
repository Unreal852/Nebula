using Nebula.Common.Medias;
using Nebula.Common.Playlist;

namespace Nebula.Services.Abstractions;

public interface IMediasProviderService
{
    Guid ServiceId { get; }
    string ServiceName { get; }
    string ServiceIcon { get; }
    string ServiceIconColor { get; }

    Task<IMediaInfo?> GetMediaAsync(string url);
    Task<Playlist?> GetPlaylistAsync(string url);
    IAsyncEnumerable<IMediaInfo> SearchMediasAsync(string searchQuery, int maxResults = 20);
}