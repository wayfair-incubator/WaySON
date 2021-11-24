using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WaySON.Converters
{
    /// <summary>
    ///    A custom <see cref="JsonConverter{T}"/> for <see cref="Dictionary{TKey,TValue}"/> where TKey is <see cref="long"/> and TValue is <see cref="string"/>.
    ///    Supports <see cref="long"/> keys represented as <see cref="string"/>.
    /// </summary>
    internal sealed class DictionaryLongStringConverter : JsonConverter<Dictionary<long, string>>
    {
        /// <inheritdoc/>
        public override Dictionary<long, string> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Malformed JSON: Expected {JsonTokenType.StartObject}, found {reader.TokenType}.");
            }

            var value = new Dictionary<long, string>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return value;
                }

                var keyAsString = reader.GetString();

                if (!long.TryParse(keyAsString, out var keyAsLong))
                {
                    throw new JsonException($"Unable to convert \"{keyAsString}\" to System.Int64.");
                }

                reader.Read();
                var itemValue = reader.GetString();

                value.Add(keyAsLong, itemValue);
            }

            throw new JsonException($"Malformed JSON: No {JsonTokenType.EndObject} token found.");
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, Dictionary<long, string> value, JsonSerializerOptions options)
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
