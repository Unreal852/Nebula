using System;
using Nebula.Media.Events;

namespace Nebula.Media
{
    public interface IPlaylist
    {
        int      Id            { get; set; }
        string   Name          { get; set; }
        string   Description   { get; set; }
        string   Author        { get; set; }
        Uri      Thumbnail     { get; set; }
        int      MediasCount   { get; }
        TimeSpan TotalDuration { get; }

        event EventHandler<PlaylistMediaAddedEventArgs>   MediaAdded;
        event EventHandler<PlaylistMediaRemovedEventArgs> MediaRemoved;
    }
}