namespace Nebula.Model
{
    public class PlaylistMediaInfo
    {
        public string PlaylistId { get; set; }
        public string MediaId    { get; set; }
        public bool   IsActive   { get; set; }
        public int    Order      { get; set; }

        public void ApplyTo(MediaInfo mediaInfo)
        {
            mediaInfo.IsActive = IsActive;
        }
    }
}