using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WaySON.Converters
{
    /// <summary>
    ///     A custom <see cref="JsonConverter{T}"/> for <see cref="double"/>.
    ///     Supports <see cref="double"/> values specified as <see cref="string"/>.
    /// </summary>
    internal sealed class DoubleConverter : JsonConverter<double>
    {
        /// <inheritdoc/>
        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();

                if (double.TryParse(stringValue, out var value))
                {
                    return value;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetDouble();
            }

            throw new JsonException($"JSON was not a valid {typeof(double)} or string representation of {typeof(double)}.");
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
