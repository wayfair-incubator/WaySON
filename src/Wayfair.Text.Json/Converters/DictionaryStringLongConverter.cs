using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wayfair.Text.Json.Converters
{
    /// <summary>
    ///     A custom <see cref="JsonConverter{T}"/> for <see cref="Dictionary{TKey,TValue}"/> where TKey is <see cref="string"/> and TValue is <see cref="long"/>.
    ///     Supports <see cref="long"/> values specified as <see cref="string"/>.
    /// </summary>
    internal sealed class DictionaryStringLongConverter : JsonConverter<Dictionary<string, long>>
    {
        /// <inheritdoc/>
        public override Dictionary<string, long> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Malformed JSON: Expected {JsonTokenType.StartObject}, found {reader.TokenType}.");
            }

            var value = new Dictionary<string, long>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return value;
                }

                var key = reader.GetString();

                reader.Read();
                var itemAsString = reader.GetString();

                if (!long.TryParse(itemAsString, out var itemAsLong))
                {
                    throw new JsonException($"Unable to convert \"{itemAsString}\" to System.Int64.");
                }

                value.Add(key, itemAsLong);
            }

            throw new JsonException($"Malformed JSON: No {JsonTokenType.EndObject} token found.");
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, Dictionary<string, long> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var pair in value)
            {
                writer.WritePropertyName(pair.Key);
                writer.WriteNumberValue(pair.Value);
            }

            writer.WriteEndObject();
        }
    }
}
