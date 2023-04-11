namespace Nebula.Services.Contracts;

public interface IAppService
{
    string GetAppVersion();

    void Shutdown();
}