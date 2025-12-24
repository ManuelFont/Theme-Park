using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ParqueTematico.WebApi.Converters;

public class ParqueDateTimeConverter : JsonConverter<DateTime>
{
    private const string Formato = "yyyy-MM-ddTHH:mm";

    public override DateTime Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return DateTime.ParseExact(
            value!,
            Formato,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Formato));
    }
}
