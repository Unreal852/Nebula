using System;
using System.Threading.Tasks;

namespace Nebula.Media
{
    /// <summary>
    /// Provide Media infos to implement by media provider results.
    /// </summary>
    public interface IMediaInfo
    {
        /// <summary>
        /// Media ID
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Media Owner Id ( Channel, Profile )
        /// </summary>
        string AuthorId { get; }

        /// <summary>
        /// Media Name
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Media Author
        /// </summary>
        string Author { get; set; }

        /// <summary>
        /// Media Description
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Low res Thumbnail Url
        /// </summary>
        string LowResThumbnailUrl { get; set; }

        /// <summary>
        /// Medium res thumbnail Url
        /// </summary>
        string MediumResThumbnailUrl { get; set; }

        // High res Thumbnail Url
        string HighResThumbnailUrl { get; set; }
        
        /// <summary>
        /// Playlist Specific field
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Media Total Duration
        /// </summary>
        TimeSpan Duration { get; set; }

        /// <summary>
        /// Media Creation Date
        /// </summary>
        DateTime CreationDate { get; }

        /// <summary>
        /// Media Provider
        /// </summary>
        /// <returns>Media Provider</returns>
        IMediasProvider GetMediaProvider();

        /// <summary>
        /// Returns media's artist's info
        /// </summary>
        /// <returns>IArtistInfo</returns>
        Task<IArtistInfo> GetArtistInfo();

        /// <summary>
        /// Returns media stream url
        /// </summary>
        /// <returns>Uri</returns>
        Task<Uri> GetAudioStreamUri();

        /// <summary>
        /// Returns media video stream uri
        /// </summary>
        /// <returns>Uri</returns>
        Task<Uri> GetVideoStreamUri();

        /// <summary>
        /// Returns media audi and video stream uri
        /// Note: Support Muxed must be true
        /// </summary>
        /// <returns>Uri</returns>
        Task<Uri> GetMuxedStreamUri();
    }
}