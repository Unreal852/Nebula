using Nebula.Common.Medias;
using Nebula.Common.Playlist;
using Nebula.Desktop.Contracts;
using Serilog;
using SerilogTimings.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Nebula.Desktop.Services.Medias;

public sealed class LocalMediasProviderService : IMediasProviderService
{
    private readonly ILogger _logger;
    private readonly ISettingsService _settingsService;
    private readonly Uri _thumbnailUri = new("https://i.imgur.com/4oV9KuT.png");

    public LocalMediasProviderService(ILogger logger, ISettingsService settingsService)
    {
        _logger = logger.WithContext(nameof(LocalMediasProviderService));
        _settingsService = settingsService;
    }

    public Guid ServiceId { get; } = new("3D35C23B-646F-4E80-8FAD-70A8D99DFD5D");

    public string ServiceName => "Local";

    public string ServiceIcon { get; }
        = "M 15.195312 3.066406 L 7.472656 3.066406 L 7.472656 1.738281 C 7.472656 1.292969 7.109375 0.933594 6.667969 0.933594 L 0.804688 0.933594 C 0.359375 0.933594 0 1.292969 0 1.738281 L 0 14.261719 C 0 14.707031 0.359375 15.066406 0.804688 15.066406 L 15.195312 15.066406 C 15.636719 15.066406 16 14.707031 16 14.261719 L 16 3.871094 C 16 3.425781 15.640625 3.066406 15.195312 3.066406 Z M 15.464844 14.261719 C 15.464844 14.410156 15.34375 14.535156 15.195312 14.535156 L 0.804688 14.535156 C 0.65625 14.535156 0.535156 14.410156 0.535156 14.261719 L 0.535156 6 L 15.464844 6 Z M 0.535156 5.464844 L 0.535156 1.738281 C 0.535156 1.589844 0.65625 1.464844 0.804688 1.464844 L 6.667969 1.464844 C 6.816406 1.464844 6.9375 1.589844 6.9375 1.738281 L 6.9375 3.601562 L 15.195312 3.601562 C 15.34375 3.601562 15.464844 3.722656 15.464844 3.871094 L 15.464844 5.464844 Z M 0.535156 5.464844 M 5.066406 8.933594 L 8.800781 8.933594 C 8.945312 8.933594 9.066406 8.8125 9.066406 8.667969 C 9.066406 8.519531 8.945312 8.398438 8.800781 8.398438 L 5.066406 8.398438 C 4.917969 8.398438 4.800781 8.519531 4.800781 8.667969 C 4.800781 8.8125 4.917969 8.933594 5.066406 8.933594 Z M 5.066406 8.933594 M 5.066406 10.535156 L 10.933594 10.535156 C 11.082031 10.535156 11.199219 10.414062 11.199219 10.265625 C 11.199219 10.121094 11.082031 10 10.933594 10 L 5.066406 10 C 4.917969 10 4.800781 10.121094 4.800781 10.265625 C 4.800781 10.414062 4.917969 10.535156 5.066406 10.535156 Z M 5.066406 10.535156 M 5.066406 12.132812 L 10.933594 12.132812 C 11.082031 12.132812 11.199219 12.015625 11.199219 11.867188 C 11.199219 11.71875 11.082031 11.601562 10.933594 11.601562 L 5.066406 11.601562 C 4.917969 11.601562 4.800781 11.71875 4.800781 11.867188 C 4.800781 12.015625 4.917969 12.132812 5.066406 12.132812 Z M 5.066406 12.132812";

    public string ServiceIconColor => "White";

    public Task<IMediaInfo?> GetMediaAsync(string url)
    {
        throw new NotSupportedException();
    }

    public Task<Playlist?> GetPlaylistAsync(string url)
    {
        throw new NotImplementedException();
    }

#pragma warning disable CS1998 // Cette méthode async n'a pas d'opérateur 'await' et elle s'exécutera de façon synchrone
    public async IAsyncEnumerable<IMediaInfo> SearchMediasAsync(string searchQuery, int maxResults = 20)
#pragma warning restore CS1998 // Cette méthode async n'a pas d'opérateur 'await' et elle s'exécutera de façon synchrone
    {
        if (!string.IsNullOrWhiteSpace(_settingsService.Settings.LocalLibraryPath))
        {
            using (_logger.TimeOperation(
                           "Searching in the local library... Query: '{Query}' MaxResults: '{MaxResults}'", searchQuery,
                           maxResults))
            {
                foreach (var file in Directory.GetFiles(_settingsService.Settings.LocalLibraryPath, "*", SearchOption.AllDirectories))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (fileName.Contains(searchQuery, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var media = new MediaInfo()
                        {
                            Id = file,
                            Title = fileName,
                            //Thumbnail = _thumbnailUri,
                            StreamUri = file,
                        };
                        yield return media;
                    }
                }
            }
        }
        else
            yield return MediaInfo.Empty;
    }
}