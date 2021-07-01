using SQLite;

namespace Nebula.Model
{
    [Table("PlaylistsMedias")]
    public class PlaylistMediaInfo
    {
        [NotNull] public int    PlaylistIndex { get; set; }
        [NotNull] public string MediaId       { get; set; }
        public           bool   IsActive      { get; set; }
        public           int    MediaOrder    { get; set; }

        public void ApplyTo(MediaInfo mediaInfo)
        {
            mediaInfo.IsActive = IsActive;
        }
    }
}