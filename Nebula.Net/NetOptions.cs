namespace Nebula.Net;

public sealed record NetOptions(string ServerAddress, int ServerPort, int ServerSlots, string ServerPassword, bool UseUpnp)
{
    public bool HasPassword => !string.IsNullOrWhiteSpace(ServerPassword);
}