using System.Collections.Generic;

namespace Nebula.Media
{
    public interface IArtistInfo : IThumbnailSource
    {
        /// <summary>
        ///     Artist Id
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     Artist Nape
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Artist Url
        /// </summary>
        public string Url { get; }

        /// <summary>
        ///     Returns Artist's Medias
        /// </summary>
        /// <returns>IMediaInfo</returns>
        public IAsyncEnumerable<IMediaInfo> GetMedias();
    }
}