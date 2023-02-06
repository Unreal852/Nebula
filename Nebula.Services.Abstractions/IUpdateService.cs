using Nebula.Common;

namespace Nebula.Services.Abstractions;

public interface IUpdateService
{
    Task<UpdateInfo> CheckForUpdates(string currentVersion);
    Task             DownloadUpdate(UpdateInfo updateInfo);
}