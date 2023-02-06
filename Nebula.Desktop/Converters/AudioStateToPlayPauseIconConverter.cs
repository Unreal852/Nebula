using System;
using System.Globalization;
using Avalonia.Data.Converters;
using FluentAvalonia.UI.Controls;
using Nebula.Common.Audio;

namespace Nebula.Desktop.Converters;

public sealed class AudioStateToPlayPauseIconConverter : IValueConverter
{
    private static readonly SymbolIcon PlayIcon  = new() { Symbol = Symbol.PlayFilled };
    private static readonly SymbolIcon PauseIcon = new() { Symbol = Symbol.PauseFilled };

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is AudioServiceState.Playing)
            return PauseIcon;
        return PlayIcon;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}