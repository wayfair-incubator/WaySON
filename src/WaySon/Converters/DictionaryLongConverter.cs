using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WaySON.Converters
{
    /// <summary>
    ///     A custom <see cref="JsonConverter{T}"/> for <see cref="Dictionary{TKey,TValue}"/> where TKey is <see cref="long"/> and TValue is any type.
    ///     Supports <see cref="long"/> keys represented as <see cref="string"/>.
    ///     Adapted from: https://github.com/dotnet/corefx/blob/master/src/System.Text.Json/tests/Serialization/CustomConverterTests.DictionaryEnumConverter.cs
    /// </summary>
    internal sealed class DictionaryLongConverter : JsonConverterFactory
    {
        /// <inheritdoc/>
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

            var genericArguments = typeToConvert.GetGenericArguments();

            return genericArguments[0] == typeof(long) &&
                   genericArguments[1] != typeof(string);
        }

        /// <inheritdoc />
        public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
        {
            var valueType = type.GetGenericArguments()[1];

            var converter = (JsonConverter)Activator.CreateInstance(
                typeof(DictionaryLongConverterInner<,>).MakeGenericType(typeof(long), valueType),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null);

            return converter;
        }

        /// <inheritdoc />
        private class DictionaryLongConverterInner<TKey, TValue> : JsonConverter<Dictionary<long, TValue>>
        {
            private readonly JsonConverter<TValue> _valueConverter;
            private readonly Type _valueType;

            public DictionaryLongConverterInner(JsonSerializerOptions options)
            {
                // For performance, use the existing converter if available.
                _valueConverter = (JsonConverter<TValue>)options.GetConverter(typeof(TValue));

                _valueType = typeof(TValue);
            }

            /// <inheritdoc />
            public override Dictionary<long, TValue> Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException($"Malformed JSON: Expected {JsonTokenType.StartObject}, found {reader.TokenType}.");
                }

                var valueDict = new Dictionary<long, TValue>();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return valueDict;
                    }

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException(
                            $"Malformed JSON: Expected {JsonTokenType.PropertyName} token type, found {reader.TokenType}.");
                    }

                    var propertyName = reader.GetString();

                    if (!long.TryParse(propertyName, out var key))
                    {
                        throw new JsonException($"Unable to convert \"{propertyName}\" to System.Int64.");
                    }

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
                Dictionary<long, TValue> value,
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
