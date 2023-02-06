namespace Nebula.Common.Medias;

public interface IMediaInfo
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Id { get; set; }
    public string? StreamUri { get; set; }
    public string? Thumbnail { get; set; }
    public Guid ProviderId { get; set; }
    public double Duration { get; set; }

    public bool HasValidId()
    {
        return !string.IsNullOrWhiteSpace(Id);
    }

    public bool HasValidStreamUri()
    {
        return StreamUri is { };
    }
}
