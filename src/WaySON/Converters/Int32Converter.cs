using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WaySON.Converters
{
    /// <summary>
    ///     A custom <see cref="JsonConverter{T}"/> for <see cref="int"/> values.
    ///     Provides support for quoted <see cref="int"/> values.
    /// </summary>
    internal sealed class Int32Converter : JsonConverter<int>
    {
        /// <inheritdoc />
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();

                if (int.TryParse(stringValue, out var value))
                {
                    return value;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32();
            }

            throw new JsonException($"JSON was not a valid {typeof(int)} or string representation of {typeof(int)}");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
