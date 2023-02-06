using System.Linq;
using Avalonia.Data.Converters;

namespace Nebula.Desktop.Converters;

public static class CustomBoolConverters
{
    public static readonly IMultiValueConverter ClientConnectedAndNotHost =
            new FuncMultiValueConverter<bool, bool>(x =>
            {
                bool[] boolValues = x.ToArray();
                if(!boolValues[0])
                    return true;
                return !boolValues[0] || boolValues[1];
            });
}