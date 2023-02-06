namespace Nebula.Services.Abstractions;

public interface IAppService
{
    string GetAppVersion();

    void Shutdown();
}