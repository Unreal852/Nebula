using Mono.Nat;

namespace Nebula.Net.Nat;

public sealed class NatMapping
{
    public required Mapping Mapping { get; init; }
    public required INatDevice NatDevice { get; init; }
}
