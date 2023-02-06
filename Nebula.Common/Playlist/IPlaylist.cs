using Nebula.Common.Medias;

namespace Nebula.Common.Playlist;

public interface IPlaylist
{
    public string Name { get; set; }
    public string? Author { get; set; }
    public string? Description { get; set; }
    public IList<MediaInfo> Medias { get; }
}
