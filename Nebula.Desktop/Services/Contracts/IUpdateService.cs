using Nebula.Common;

namespace Nebula.Services.Contracts;

public interface IUpdateService
{
    Task<UpdateInfo> CheckForUpdates(string currentVersion);
    Task DownloadUpdate(UpdateInfo updateInfo);
}