namespace Nebula.Core
{
    public interface IThumbnailSource
    {
        /// <summary>
        ///     Low Resolution Thumbnail
        /// </summary>
        public string LowResThumbnail { get; }

        /// <summary>
        ///     Medium Resolution Thumbnail
        /// </summary>
        public string MediumResThumbnail { get; }

        /// <summary>
        ///     High Resolution Thumbnail
        /// </summary>
        public string HighResThumbnail { get; }

        /// <summary>
        ///     Custom user defined thumbnail
        /// </summary>
        public string CustomThumbnail { get; }

        /// <summary>
        ///     Return the first existing thumbnail from the highest to the lowest.
        ///     <remarks>The <see cref="CustomThumbnail" /> is the first returned if the value is not null or empty</remarks>
        /// </summary>
        public string AnyThumbnailFromHighest { get; }

        /// <summary>
        ///     Return the first existing thumbnail from the lowest to the highest
        ///     <remarks>The <see cref="CustomThumbnail" /> is the first returned if the value is not null or empty</remarks>
        /// </summary>
        public string AnyThumbnailFromLowest { get; }
    }
}