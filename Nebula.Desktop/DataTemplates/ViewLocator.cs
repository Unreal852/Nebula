using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Nebula.Desktop.ViewModel;
using Serilog;

namespace Nebula.Desktop.DataTemplates;

public sealed class ViewLocator : IDataTemplate
{
    private readonly ILogger _logger;

    public ViewLocator(ILogger logger)
    {
        _logger = logger;
    }

    public Control Build(object? data)
    {
        if (data == null)
            return NotFoundView("Not found, data is null");

        string name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type != null)
        {
            object? instance = Activator.CreateInstance(type);
            if (instance == null)
                return NotFoundView($"Not found: {name}");
            return (Control)instance;
        }

        return NotFoundView($"Not found: {name}");
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }

    private static Control NotFoundView(string message)
    {
        return new TextBlock
        {
                Text = message,
                Foreground = Brushes.Red,
                FontSize = 24,
                FontWeight = FontWeight.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
        };
    }
}