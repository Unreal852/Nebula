namespace Nebula.Desktop.Contracts;

public interface IAppService
{
    string GetAppVersion();

    void Shutdown();
}