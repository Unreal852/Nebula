using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nebula.Media
{
    public interface IMediasProvider
    {
        /// <summary>
        ///     Media Provider Url
        /// </summary>
        public string Url { get; }

        /// <summary>
        ///     Media Provider Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Media Provider Color
        /// </summary>
        public string NameColorEx { get; }

        /// <summary>
        ///     Search medias
        /// </summary>
        /// <param name="query">Media Query, usually keywords</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>
        ///     <see cref="IAsyncEnumerable{T}" />
        /// </returns>
        IAsyncEnumerable<IMediaInfo> SearchMedias(string query, params object[] args);

        /// <summary>
        ///     Get Artist's medias
        /// </summary>
        /// <param name="query">Media query, usually artist's Id</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>
        ///     <see cref="IEnumerable{T}" />
        /// </returns>
        IAsyncEnumerable<IMediaInfo> GetArtistMedias(string query, params object[] args);

        /// <summary>
        ///     Get Media info
        /// </summary>
        /// <param name="query">Media query, usually media Id or Url</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>
        ///     <see cref="IMediaInfo" />
        /// </returns>
        Task<IMediaInfo> GetMediaInfo(string query, params object[] args);

        /// <summary>
        ///     Get Artist info
        /// </summary>
        /// <param name="query">Artist Query, usually artist's Id</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>
        ///     <see cref="IArtistInfo" />
        /// </returns>
        Task<IArtistInfo> GetArtistInfo(string query, params object[] args);

        /// <summary>
        ///     Get playlist
        /// </summary>
        /// <param name="query">Playlist query, usually playlist's id</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>
        ///     <see cref="IPlaylist" />
        /// </returns>
        Task<IPlaylist> GetPlaylist(string query, params object[] args);

        /// <summary>
        ///     Get the audio stream from the specified media
        /// </summary>
        /// <param name="mediaInfo">The media to retrieve the stream from</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>
        ///     <see cref="Uri" />
        /// </returns>
        Task<Uri> GetAudioStreamUri(IMediaInfo mediaInfo, params object[] args);

        /// <summary>
        ///     Get the muxed stream ( audio and video ) from the specified media
        /// </summary>
        /// <param name="mediaInfo">The media to retrieve the stream from</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>
        ///     <see cref="Uri" />
        /// </returns>
        Task<Uri> GetMuxedStreamUri(IMediaInfo mediaInfo, params object[] args);

        /// <summary>
        ///     Get the video stream from the specified media
        /// </summary>
        /// <param name="mediaInfo">The media to retrieve the stream from</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>
        ///     <see cref="Uri" />
        /// </returns>
        Task<Uri> GetVideoStreamUri(IMediaInfo mediaInfo, params object[] args);
    }
}