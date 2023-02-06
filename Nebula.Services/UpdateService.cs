using System.Diagnostics;
using Nebula.Common;
using Nebula.Common.Extensions;
using Nebula.Services.Abstractions;
using Octokit;
using Semver;
using Serilog;

namespace Nebula.Services;

public class UpdateService : IUpdateService
{
    private const    string     RepositoryOwner = "Unreal852";
    private const    string     RepositoryName  = "Nebula";
    private readonly HttpClient _httpClient;
    private readonly ILogger    _logger;

    public UpdateService(ILogger logger)
    {
        _logger = logger.WithPrefix(nameof(UpdateService));
        _httpClient = new(new SocketsHttpHandler
                { PooledConnectionLifetime = TimeSpan.FromMinutes(1) });
        var product = new ProductHeaderValue(RepositoryName);
        var apiConn = new ApiConnection(new Connection(product));
        GithubClient = new RepositoriesClient(apiConn);
    }

    private RepositoriesClient GithubClient { get; }

    public async Task<UpdateInfo> CheckForUpdates(string currentVersion)
    {
        // TODO: Try catch
        var release = await GetLatestRelease();

        if (release == null)
            return UpdateInfo.UpToDate;

        var remoteVersion = ParseVersion(release.TagName);

        if (release.Assets.Count == 0)
        {
            _logger.Warning("The release '{ReleaseVersion}' has no assets", remoteVersion.ToString());
            return UpdateInfo.UpToDate;
        }

        var localVersion = ParseVersion(currentVersion);

        return remoteVersion.ComparePrecedenceTo(localVersion) switch
        {
                1 => new UpdateInfo
                {
                        NewVersion = remoteVersion.ToString(), AssetUrl = release.Assets[0].BrowserDownloadUrl,
                        UpdateAvailable = true
                },
                _ => UpdateInfo.UpToDate
        };
    }

    public async Task DownloadUpdate(UpdateInfo updateInfo)
    {
        var updateFile = $"{Path.GetTempFileName()}.exe";
        await using var fs = File.OpenWrite(updateFile);
        await using var remoteStream = await _httpClient.GetStreamAsync(updateInfo.AssetUrl);
        await remoteStream.CopyToAsync(fs);
        await remoteStream.DisposeAsync();
        await fs.DisposeAsync();
        Process.Start(new ProcessStartInfo(updateFile)
                { UseShellExecute = true });
    }

    private async Task<Release?> GetLatestRelease()
    {
        try
        {
            var release = await GithubClient.Release.GetLatest(RepositoryOwner, RepositoryName);
            if (release != null)
                return release;
            _logger.Warning("No release found");
            return default;
        }
        catch (Exception e)
        {
            _logger.Warning(e, "Failed to fetch latest release");
            return default;
        }
    }

    private SemVersion ParseVersion(string version)
    {
        return SemVersion.Parse(version,
                SemVersionStyles.Strict | SemVersionStyles.AllowV);
    }
}