using Realms;

namespace Nebula.Common.Medias;

public sealed partial class MediaInfo : IMediaInfo, IEmbeddedObject
{
    public static IMediaInfo Empty { get; } = new MediaInfo
            { Title = "Empty", Author = "Empty", Id = "0", Duration = 0, StreamUri = null };

    public static IMediaInfo FromId(string id) => new MediaInfo { Id = id };

    public string? Title      { get; set; }
    public string? Author     { get; set; }
    public string? Id         { get; set; }
    public string? StreamUri  { get; set; }
    public string? Thumbnail  { get; set; }
    public double  Duration   { get; set; } = 0;
    public Guid    ProviderId { get; set; }

    public bool HasValidId()
    {
        return !string.IsNullOrWhiteSpace(Id);
    }

    public bool HasValidStreamUri()
    {
        return StreamUri is { };
    }
}