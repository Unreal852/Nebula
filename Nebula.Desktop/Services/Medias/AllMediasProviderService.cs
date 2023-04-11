using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Nebula.Common.Medias;
using Nebula.Common.Playlist;
using Nebula.Services.Contracts;

namespace Nebula.Services.Medias;

public sealed partial class AllMediasProviderService : ObservableObject, IMediasProviderService
{
    private readonly ILanguageService _languageService;

    [ObservableProperty]
    private string _serviceName;

    public AllMediasProviderService(ILanguageService languageService)
    {
        _languageService = languageService;
        _languageService.LanguageChanged += OnAppLanguageChanged;
        _serviceName = _languageService.GetString("MediasProviderAll");
    }

    private void OnAppLanguageChanged(object? sender, Common.Localization.LanguageChangedEventArgs e)
    {
        ServiceName = _languageService.GetString("MediasProviderAll");
    }

    public Guid ServiceId { get; } = new("E716D69E-2DEE-412B-A130-40DE11B1EC6C");

    public string ServiceIcon { get; }
        = "M379.749,124.778c-3.009-0.226-6.019-0.342-8.287-0.308c-0.914-0.105-96.472-7.101-134.731,118.827    c-33.442,110.086-110.914,105.187-115.01,104.889c-1.529,0-3.067-0.033-4.606-0.105c-54.625-2.423-97.423-47.193-97.423-101.923    c0-56.26,45.769-102.034,102.029-102.034c34.875,0,66.98,17.539,85.884,46.918l16.558-10.654    c-22.539-35.039-60.836-55.957-102.442-55.957C54.606,124.432,0,179.037,0,246.158c0,65.293,51.057,118.707,116.231,121.596    c1.817,0.077,3.634,0.12,4.615,0.087c0.096,0.01,1.25,0.096,3.27,0.096c17.182,0,97.221-6.231,131.461-118.919    c33.442-110.086,110.933-105.182,115.01-104.894c2.587,0,5.163,0.101,7.702,0.289c52.895,3.942,94.327,48.63,94.327,101.745    c0,56.26-45.769,102.029-102.029,102.029c-34.875,0-66.972-17.538-85.875-46.918l-16.557,10.654    c22.538,35.039,60.827,55.957,102.432,55.957c67.115,0,121.721-54.606,121.721-121.721    C492.308,182.793,442.865,129.475,379.749,124.778z";

    public string ServiceIconColor { get; } = "DodgerBlue";

    public Task<IMediaInfo?> GetMediaAsync(string url)
    {
        throw new NotSupportedException();
    }

    public Task<Playlist?> GetPlaylistAsync(string url)
    {
        throw new NotImplementedException();
    }

    public async IAsyncEnumerable<IMediaInfo> SearchMediasAsync(string searchQuery, int maxResults = 20)
    {
        foreach (var service in Ioc.Default.GetService<IEnumerable<IMediasProviderService>>()!)
        {
            if (service == this)
                continue;
            await foreach (var mediaInfo in service.SearchMediasAsync(searchQuery, maxResults))
                yield return mediaInfo;
        }
    }
}