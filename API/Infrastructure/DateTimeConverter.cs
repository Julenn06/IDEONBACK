using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IdeonBack.API.Infrastructure;

/// <summary>
/// Convertidor personalizado de DateTime que soporta múltiples formatos
/// </summary>
public class FlexibleDateTimeConverter : JsonConverter<DateTime?>
{
    private readonly string[] _formats = new[]
    {
        "yyyy-MM-dd HH:mm:ss.fff",
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-ddTHH:mm:ss.fffZ",
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-ddTHH:mm:ss.fff",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-dd"
    };

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (string.IsNullOrWhiteSpace(stringValue))
                return null;

            // Intentar parsear con múltiples formatos
            foreach (var format in _formats)
            {
                if (DateTime.TryParseExact(stringValue, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                {
                    // Convertir a UTC para compatibilidad con PostgreSQL
                    return DateTime.SpecifyKind(result, DateTimeKind.Utc);
                }
            }

            // Intentar parseo general
            if (DateTime.TryParse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var generalResult))
            {
                // Convertir a UTC para compatibilidad con PostgreSQL
                return DateTime.SpecifyKind(generalResult, DateTimeKind.Utc);
            }

            throw new JsonException($"No se pudo convertir '{stringValue}' a DateTime");
        }

        throw new JsonException("Formato de fecha inválido");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture));
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
