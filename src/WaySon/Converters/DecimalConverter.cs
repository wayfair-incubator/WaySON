using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WaySON.Converters
{
    /// <summary>
    ///     A custom <see cref="JsonConverter{T}"/> for <see cref="decimal"/>.
    ///     Decimals can be in any valid <see cref="decimal"/> or <see cref="string"/> representation of decimal.
    /// </summary>
    internal sealed class DecimalConverter : JsonConverter<decimal>
    {
        /// <inheritdoc />
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();

                if (decimal.TryParse(stringValue, out var value))
                {
                    return value;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetDecimal();
            }

            throw new JsonException("JSON was not a valid decimal or string representation of decimal.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
