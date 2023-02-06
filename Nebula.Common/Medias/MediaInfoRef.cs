namespace Nebula.Common.Medias;

public readonly struct MediaInfoRef : IMediaInfo
{
    public static IMediaInfo Empty => throw new NotImplementedException();
    public static IMediaInfo FromId(string id) => throw new NotImplementedException();

    private readonly IMediaInfo _reference;

    public MediaInfoRef(IMediaInfo reference)
    {
        _reference = reference;
    }

    public IMediaInfo Reference => _reference;

    public string? Id
    {
        get => _reference.Id;
        set => _reference.Id = value;
    }

    public string? Title
    {
        get => _reference.Title;
        set => _reference.Title = value;
    }

    public string? Author
    {
        get => _reference.Author;
        set => _reference.Author = value;
    }

    public string? StreamUri
    {
        get => _reference.StreamUri;
        set => _reference.StreamUri = value;
    }

    public string? Thumbnail
    {
        get => _reference.Thumbnail;
        set => _reference.Thumbnail = value;
    }

    public double Duration
    {
        get => _reference.Duration;
        set => _reference.Duration = value;
    }

    public Guid ProviderId
    {
        get => _reference.ProviderId;
        set => _reference.ProviderId = value;
    }
}
