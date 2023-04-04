using Nebula.Common.Medias;
using Realms;

namespace Nebula.Common.Playlist;

public partial class Playlist : IPlaylist, IRealmObject
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Author { get; set; }
    public IList<MediaInfo> Medias { get; } = default!;
}