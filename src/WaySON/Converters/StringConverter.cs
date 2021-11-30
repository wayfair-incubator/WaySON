using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WaySON.Converters
{
    /// <summary>
    ///     A custom <see cref="JsonConverter{T}"/> for <see cref="string"/> values.
    ///     Adds to <see cref="System.Text.Json"/> by allowing non-quoted numbers in the JSON.
    /// </summary>
    internal sealed class StringConverter : JsonConverter<string>
    {
        /// <inheritdoc/>
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var type = reader.TokenType;

            if (type == JsonTokenType.Number)
            {
                if (reader.TryGetInt32(out var intValue))
                {
                    return intValue.ToString();
                }

                if (reader.TryGetDouble(out var doubleValue))
                {
                    return doubleValue.ToString();
                }
            }

            return reader.GetString();
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
