namespace Nebula.Core.Extensions
{
    public static class ThumbnailSourceExtensions
    {
        /// <summary>
        ///     Return the first existing thumbnail from the highest to the lowest.
        ///     <remarks>The <see cref="IThumbnailSource.CustomThumbnail" /> is the first returned if the value is not null or empty</remarks>
        /// </summary>
        public static string AnyThumbnailFromHighest(this IThumbnailSource source)
        {
            if (!string.IsNullOrWhiteSpace(source.CustomThumbnail))
                return source.CustomThumbnail;
            if (!string.IsNullOrWhiteSpace(source.HighResThumbnail))
                return source.HighResThumbnail;
            if (!string.IsNullOrWhiteSpace(source.MediumResThumbnail))
                return source.MediumResThumbnail;
            if (!string.IsNullOrWhiteSpace(source.LowResThumbnail))
                return source.LowResThumbnail;
            return "https://i.imgur.com/Od5XogD.png";
        }

        /// <summary>
        ///     Return the first existing thumbnail from the lowest to the highest
        ///     <remarks>The <see cref="IThumbnailSource.CustomThumbnail" /> is the first returned if the value is not null or empty</remarks>
        /// </summary>
        public static string AnyThumbnailFromLowest(this IThumbnailSource source)
        {
            if (!string.IsNullOrWhiteSpace(source.CustomThumbnail))
                return source.CustomThumbnail;
            if (!string.IsNullOrWhiteSpace(source.LowResThumbnail))
                return source.LowResThumbnail;
            if (!string.IsNullOrWhiteSpace(source.MediumResThumbnail))
                return source.MediumResThumbnail;
            if (!string.IsNullOrWhiteSpace(source.HighResThumbnail))
                return source.HighResThumbnail;
            return "https://i.imgur.com/Od5XogD.png";
        }
    }
}