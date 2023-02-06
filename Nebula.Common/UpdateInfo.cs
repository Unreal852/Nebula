namespace Nebula.Common;

public class UpdateInfo
{
    public static readonly UpdateInfo UpToDate = new() { UpdateAvailable = false };

    public string? NewVersion      { get; init; }
    public string? AssetUrl        { get; init; }
    public string? Message         { get; init; }
    public bool    UpdateAvailable { get; init; }
}