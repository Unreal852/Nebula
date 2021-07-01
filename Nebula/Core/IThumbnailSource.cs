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
    }
}