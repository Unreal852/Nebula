using SQLite;

namespace Nebula.Model
{
    [Table("PlaylistsMedias")]
    public class PlaylistMediaInfo
    {
        [NotNull] public string PlaylistId { get; set; }
        [NotNull] public string MediaId    { get; set; }
        public           bool   IsActive   { get; set; }
        public           int    Order      { get; set; }

        public void ApplyTo(MediaInfo mediaInfo)
        {
            mediaInfo.IsActive = IsActive;
        }
    }
}