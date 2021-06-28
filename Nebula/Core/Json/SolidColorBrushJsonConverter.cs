using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Nebula.Core.Json
{
    public class SolidColorBrushJsonConverter : JsonConverter<SolidColorBrush>
    {
        public override SolidColorBrush Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new((Color) ColorConverter.ConvertFromString(reader.GetString()));
        }

        public override void Write(Utf8JsonWriter writer, SolidColorBrush value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"#{value.Color.R:X2}{value.Color.G:X2}{value.Color.B:X2}");
        }
    }
}