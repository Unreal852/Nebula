using Mono.Nat;
using Nebula.Common.Extensions;
using Nebula.Net.Nat;
using Serilog;

namespace Nebula.Net.Services;

public sealed class NatMapperService : INatMapperService
{
    private readonly ILogger _logger;

    public NatMapperService(ILogger logger)
    {
        _logger = logger.WithContext(nameof(NatMapperService));
    }

    public async Task<NatMapping?> Map(Protocol protocol, int port, string mappingName)
    {
        _logger.Information("Mapping '{MappingName}' with protocol {Protocol} on port {Port} ", mappingName, protocol, port);
        using var natDiscoverer = new NatDiscoverer();
        var cancellationSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var natDevice = await natDiscoverer.DiscoverDeviceAsync(cancellationSource.Token);
        if (natDevice == null)
        {
            _logger.Warning("Nat mapping failed ! No device found.");
            return null;
        }

        try
        {
            var mapping = new Mapping(protocol, port, port, 0, mappingName);
            var mappingResult = await natDevice.CreatePortMapAsync(mapping);
            if (mappingResult == null)
            {
                _logger.Warning("Nat mapping failed !");
                return null;
            }
            _logger.Information("Successfully mapped '{MappingName}' with protocol {Protocol} on port {Port} ", mappingName, protocol, port);
            return new NatMapping { NatDevice = natDevice, Mapping = mappingResult };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Nat mapping failed");
        }
        return null;
    }

    public Task Unmap(NatMapping natMapping)
    {
        return natMapping.NatDevice.DeletePortMapAsync(natMapping.Mapping);
    }
}
