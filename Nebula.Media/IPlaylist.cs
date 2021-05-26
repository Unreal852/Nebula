using System;
using System.Collections.Generic;
using Nebula.Media.Events;
using Nebula.Media.Player;

namespace Nebula.Media
{
    public interface IPlaylist : IEnumerable<IMediaInfo>
    {
        int              Id            { get; set; }
        string           Name          { get; set; }
        string           Description   { get; set; }
        string           Author        { get; set; }
        Uri              Thumbnail     { get; set; }
        MediasCollection Medias        { get; }
        int              MediasCount   { get; }
        TimeSpan         TotalDuration { get; }

        event EventHandler<PlaylistMediaAddedEventArgs>   MediaAdded;
        event EventHandler<PlaylistMediaRemovedEventArgs> MediaRemoved;

        IMediaInfo GetMedia(int index);
        bool       Contains(IMediaInfo mediaInfo);
        void       AddMedia(IMediaInfo mediaInfo, int insertIndex = -1);
        void       AddMedias(params IMediaInfo[] mediaInfo);
        void       RemoveMedia(IMediaInfo mediaInfo);
        void       RemoveMedias(params IMediaInfo[] medias);
    }
}