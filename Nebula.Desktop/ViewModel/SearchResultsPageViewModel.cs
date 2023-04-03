using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using Nebula.Common.Medias;
using Nebula.Desktop.Properties;
using Nebula.Services.Abstractions;

namespace Nebula.Desktop.ViewModel;

public sealed partial class SearchResultsPageViewModel : ViewModelPageBase
{
    private readonly IAudioPlayerService _audioPlayerService;

    [ObservableProperty]
    private ObservableCollection<IMediaInfo> _searchResults = new();

    public SearchResultsPageViewModel(IAudioPlayerService service)
    {
        _audioPlayerService = service;
        PageName = Resources.SearchResults;
        PageIcon = Symbol.ShowResults;
        PageIsAlwaysVisible = false;
    }

    public async Task SearchMedias(string searchQuery, IMediasProviderService service)
    {
        await foreach (IMediaInfo media in service.SearchMediasAsync(searchQuery))
        {
            if (Equals(media, MediaInfo.Empty))
                continue;
            SearchResults.Add(media);
        }
    }

    [RelayCommand]
    public Task OpenMedia(IMediaInfo mediaInfo)
    {
        return _audioPlayerService.OpenMedia(mediaInfo);
    }
}