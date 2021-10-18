using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wayfair.Text.Json.Converters
{
    /// <summary>
    ///    A custom <see cref="JsonConverter{T}"/> for <see cref="Dictionary{TKey,TValue}"/> where TKey is <see cref="int"/> and TValue is <see cref="string"/>.
    ///    Supports <see cref="int"/> keys represented as <see cref="string"/>.
    /// </summary>
    internal sealed class DictionaryIntStringConverter : JsonConverter<Dictionary<int, string>>
    {
        /// <inheritdoc/>
        public override Dictionary<int, string> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Malformed JSON: Expected {JsonTokenType.StartObject}, found {reader.TokenType}.");
            }

            var value = new Dictionary<int, string>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return value;
                }

                var keyAsString = reader.GetString();

                if (!int.TryParse(keyAsString, out var keyAsInt))
                {
                    throw new JsonException($"Unable to convert \"{keyAsString}\" to System.Int32.");
                }

                reader.Read();
                var itemValue = reader.GetString();

                value.Add(keyAsInt, itemValue);
            }

            throw new JsonException($"Malformed JSON: No {JsonTokenType.EndObject} token found.");
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, Dictionary<int, string> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var item in value)
            {
                writer.WriteString(item.Key.ToString(), item.Value);
            }

            writer.WriteEndObject();
        }
    }
}
