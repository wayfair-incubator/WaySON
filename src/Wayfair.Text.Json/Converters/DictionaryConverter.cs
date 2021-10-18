using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wayfair.Text.Json.Serializers;

namespace Wayfair.Text.Json.Converters
{
    /// <summary>
    ///     A custom <see cref="JsonConverter{T}"/> for <see cref="Dictionary{TKey,TValue}"/>. Register explicitly typed Dictionaries before this.
    /// </summary>
    internal sealed class DictionaryConverter : JsonConverterFactory
    {
        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
            {
                return false;
            }

            if (typeToConvert.GetGenericTypeDefinition() != typeof(Dictionary<,>))
            {
                return false;
            }

            return typeToConvert.GetGenericArguments()[0] != typeof(string);
        }

        /// <inheritdoc />
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert is null)
            {
                throw new ArgumentNullException(nameof(typeToConvert));
            }

            var typeArgs = typeToConvert.GetGenericArguments();
            var keyType = typeArgs[0];
            var valueType = typeArgs[1];

            var converter = (JsonConverter)Activator.CreateInstance(
                typeof(DictionaryConverterInner<,>).MakeGenericType(keyType, valueType),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null
            );

            return converter;
        }

        private class DictionaryConverterInner<TKey, TValue> : JsonConverter<Dictionary<TKey, TValue>>
        {
            private readonly JsonConverter<TValue> _valueConverter;
            private readonly Type _valueType;

            public DictionaryConverterInner(JsonSerializerOptions options)
            {
                _valueConverter = (JsonConverter<TValue>)options.GetConverter(typeof(TValue));
                _valueType = typeof(TValue);
            }

            /// <inheritdoc />
            public override Dictionary<TKey, TValue> Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException($"Malformed JSON: Expected {JsonTokenType.StartObject}, found {reader.TokenType}.");
                }

                var valueDict = new Dictionary<TKey, TValue>();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return valueDict;
                    }

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException(
                            $"Malformed JSON: Expected {JsonTokenType.PropertyName}, found {reader.TokenType}."
                        );
                    }

                    var keyString = reader.GetString();
                    var key = WayfairJsonSerializer.Deserialize<TKey>(keyString);

                    TValue value;

                    if (_valueConverter != null)
                    {
                        reader.Read();
                        value = _valueConverter.Read(ref reader, _valueType, options);
                    }
                    else
                    {
                        value = JsonSerializer.Deserialize<TValue>(ref reader, options);
                    }

                    valueDict.Add(key, value);
                }

                throw new JsonException($"Malformed JSON: No {JsonTokenType.EndObject} token found.");
            }

            /// <inheritdoc />
            public override void Write(
                Utf8JsonWriter writer,
                Dictionary<TKey, TValue> value,
                JsonSerializerOptions options)
            {
                writer.WriteStartObject();

                foreach (var pair in value)
                {
                    writer.WritePropertyName(pair.Key.ToString());

                    if (_valueConverter != null)
                    {
                        _valueConverter.Write(writer, pair.Value, options);
                    }
                    else
                    {
                        JsonSerializer.Serialize(writer, pair.Value, options);
                    }
                }

                writer.WriteEndObject();
            }
        }
    }
}
