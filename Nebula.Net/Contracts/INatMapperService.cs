using Mono.Nat;
using Nebula.Net.Nat;

namespace Nebula.Net.Services;

public interface INatMapperService
{
    Task<NatMapping?> Map(Protocol protocol, int port, string mappingName);
    Task Unmap(NatMapping natMapping);
}
