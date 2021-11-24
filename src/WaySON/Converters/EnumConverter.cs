using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WaySON.Converters
{
    /// <summary>
    ///     A custom <see cref="JsonConverter{T}"/> for <see cref="Enum"/>.
    ///     Supports <see cref="Enum"/> values specified as <see cref="string"/>.
    /// </summary>
    internal sealed class EnumConverter : JsonConverterFactory
    {
        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert is null)
            {
                throw new ArgumentNullException(nameof(typeToConvert));
            }

            return typeToConvert.IsEnum;
        }

        /// <inheritdoc />
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converter = (JsonConverter)Activator.CreateInstance(
                typeof(EnumConverterInner<>).MakeGenericType(typeToConvert),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null);

            return converter;
        }

        /// <inheritdoc />
        private class EnumConverterInner<T> : JsonConverter<T>
            where T : struct, Enum
        {
            public EnumConverterInner(JsonSerializerOptions options) { }

            /// <inheritdoc />
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    var stringValue = reader.GetString();

                    if (!Enum.TryParse(stringValue, false, out T value))
                    {
                        throw new JsonException($"Unable to convert \"{stringValue}\" to Enum \"{typeToConvert}\".");
                    }

                    return value;
                }

                if (reader.TokenType == JsonTokenType.Number)
                {
                    var intValue = reader.GetInt32();

                    if (!Enum.TryParse(intValue.ToString(), false, out T value))
                    {
                        throw new JsonException($"Unable to convert \"{intValue}\" to Enum \"{typeToConvert}\".");
                    }

                    return value;
                }

                throw new JsonException($"Unable to convert to Enum \"{typeToConvert}\".");
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}
