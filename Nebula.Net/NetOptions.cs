namespace Nebula.Net;

public sealed record NetOptions(string ServerAddress, int ServerPort, int ServerSlots, bool UseUpnp)
{
    public bool IsValidForClient => !string.IsNullOrWhiteSpace(ServerAddress) && ServerPort >= 5000;
    public bool IsValidForServer => ServerPort >= 5000;
}