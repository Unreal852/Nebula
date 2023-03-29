using AsyncImageLoader;
using AsyncImageLoader.Loaders;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Nebula.Desktop.View;

public sealed partial class SearchResultsPageView : UserControl
{
    public SearchResultsPageView()
    {
        InitializeComponent();
        if (ImageLoader.AsyncImageLoader is not BaseWebImageLoader)
            ImageLoader.AsyncImageLoader = new BaseWebImageLoader();
    }
}