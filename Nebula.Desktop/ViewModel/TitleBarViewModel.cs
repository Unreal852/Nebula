using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Nebula.Desktop.ViewModel.Messages;
using Nebula.Services.Abstractions;

namespace Nebula.Desktop.ViewModel;

public sealed partial class TitleBarViewModel : ViewModelBase
{
    private const int MaxSearchSuggestions = 5;

    [ObservableProperty]
    private IMediasProviderService _currentMediasProviderService = default!;

    [ObservableProperty]
    private IReadOnlyList<IMediasProviderService> _mediasProviderServices = default!;

    [ObservableProperty]
    private ObservableCollection<string> _searchSuggestions = new();

    public TitleBarViewModel(IEnumerable<IMediasProviderService> services)
    {
        MediasProviderServices = new List<IMediasProviderService>(services).AsReadOnly();
        CurrentMediasProviderService = MediasProviderServices[0];
    }

    [RelayCommand]
    private Task SearchMedias(string searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
            return Task.CompletedTask;
        var searchPage = new SearchResultsPageViewModel(Ioc.Default.GetRequiredService<IAudioPlayerService>());
        StrongReferenceMessenger.Default.Send(new PageChangeMessage(searchPage));
        AddSuggestion(searchQuery);
        return searchPage.SearchMedias(searchQuery, CurrentMediasProviderService);
    }

    private void AddSuggestion(string searchQuery)
    {
        if (SearchSuggestions.Contains(searchQuery))
            return;
        if (SearchSuggestions.Count >= MaxSearchSuggestions)
            SearchSuggestions.RemoveAt(SearchSuggestions.Count - 1);
        SearchSuggestions.Add(searchQuery);
    }
}