using Mono.Nat;
using Serilog;

namespace Nebula.Net.Nat;

public sealed class NatDiscoverer : IDisposable
{
    private bool _isDisposed;

    private INatDevice? NatDevice { get; set; }

    public void Dispose()
    {
        if(_isDisposed)
            return;
        _isDisposed = true;
        NatUtility.DeviceFound -= OnNatDeviceFound;
    }

    public async Task<INatDevice?> DiscoverDeviceAsync(CancellationToken token)
    {
        NatUtility.DeviceFound += OnNatDeviceFound;
        NatUtility.StartDiscovery(NatProtocol.Upnp);
        try
        {
            while(NatDevice == null)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(5, token);
            }
            NatUtility.StopDiscovery();
        }
        catch(Exception e)
        {
            if(e is not OperationCanceledException)
                Log.Error(e, "[NatDiscoverer] Error while discovering nat devices");
        }

        return NatDevice;
    }

    private void OnNatDeviceFound(object? sender, DeviceEventArgs e)
    {
        NatDevice = e.Device;
    }
}