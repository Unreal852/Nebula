using Nebula.Common;
using System.Threading.Tasks;

namespace Nebula.Desktop.Contracts;

public interface IUpdateService
{
    Task<UpdateInfo> CheckForUpdates(string currentVersion);
    Task DownloadUpdate(UpdateInfo updateInfo);
}