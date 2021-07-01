using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nebula.Core.Playlists;
using Nebula.Model;

namespace Nebula.Core.Providers
{
    /// <summary>
    ///     The base class for a Media provider
    /// </summary>
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
        ///     Media Provider Type
        /// </summary>
        public ProviderType ProviderType { get; }


        /// <summary>
        ///     Get the artist url
        /// </summary>
        /// <param name="artistInfo">Artist</param>
        /// <returns>Artist Url</returns>
        string GetArtistUrl(ArtistInfo artistInfo);

        /// <summary>
        ///     Get the media url
        /// </summary>
        /// <param name="mediaInfo">Media</param>
        /// <returns>Media Url</returns>
        string GetMediaUrl(MediaInfo mediaInfo);

        /// <summary>
        ///     Get the playlist url
        /// </summary>
        /// <param name="playlistInfo">Playlist</param>
        /// <returns>Playlist Url</returns>
        string GetPlaylistUrl(Playlist playlistInfo);

        /// <summary>
        ///     Search medias
        /// </summary>
        /// <param name="query">Media Query, usually keywords</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>
        ///     <see cref="IAsyncEnumerable{T}" />
        /// </returns>
        IAsyncEnumerable<MediaInfo> SearchMedias(string query, params object[] args);

        /// <summary>
        ///     Search Playlists
        /// </summary>
        /// <param name="query">Playlist Query, usually keywords</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>
        ///     <see cref="IAsyncEnumerable{T}" />
        /// </returns>
        IAsyncEnumerable<Playlist> SearchPlaylists(string query, params object[] args);

        /// <summary>
        ///     Search Artists
        /// </summary>
        /// <param name="query">Artists Query, usually keywords</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>
        ///     <see cref="IAsyncEnumerable{T}" />
        /// </returns>
        IAsyncEnumerable<ArtistInfo> SearchArtists(string query, params object[] args);

        /// <summary>
        ///     Get Artist's medias
        /// </summary>
        /// <param name="query">Media query, usually artist's Id</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>
        ///     <see cref="IEnumerable{T}" />
        /// </returns>
        IAsyncEnumerable<MediaInfo> GetArtistMedias(string query, params object[] args);

        /// <summary>
        ///     Get Media info
        /// </summary>
        /// <param name="query">Media query, usually media Id or Url</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>
        ///     <see cref="IMediaInfo" />
        /// </returns>
        Task<MediaInfo> GetMediaInfo(string query, params object[] args);

        /// <summary>
        ///     Get Artist info
        /// </summary>
        /// <param name="query">Artist Query, usually artist's Id</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>
        ///     <see cref="IArtistInfo" />
        /// </returns>
        Task<ArtistInfo> GetArtistInfo(string query, params object[] args);

        /// <summary>
        ///     Get playlist
        /// </summary>
        /// <param name="query">Playlist query, usually playlist's id</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>
        ///     <see cref="IPlaylist" />
        /// </returns>
        Task<Playlist> GetPlaylist(string query, params object[] args);

        /// <summary>
        ///     Get the audio stream from the specified media
        /// </summary>
        /// <param name="mediaInfo">The media to retrieve the stream from</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>
        ///     <see cref="Uri" />
        /// </returns>
        Task<Uri> GetAudioStreamUri(MediaInfo mediaInfo, params object[] args);

        /// <summary>
        ///     Get the muxed stream ( audio and video ) from the specified media
        /// </summary>
        /// <param name="mediaInfo">The media to retrieve the stream from</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>
        ///     <see cref="Uri" />
        /// </returns>
        Task<Uri> GetMuxedStreamUri(MediaInfo mediaInfo, params object[] args);

        /// <summary>
        ///     Get the video stream from the specified media
        /// </summary>
        /// <param name="mediaInfo">The media to retrieve the stream from</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>
        ///     <see cref="Uri" />
        /// </returns>
        Task<Uri> GetVideoStreamUri(MediaInfo mediaInfo, params object[] args);
    }
}