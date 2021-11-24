using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WaySON.Converters
{
    /// <summary>
    ///     A custom <see cref="JsonConverter{T}"/> for <see cref="IDictionary{TKey,TValue}"/>. Register explicitly typed Dictionary converters before this.
    /// </summary>
    internal sealed class IDictionaryConverter : JsonConverterFactory
    {
        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
            {
                return false;
            }

            if (typeToConvert.GetGenericTypeDefinition() != typeof(IDictionary<,>))
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
                typeof(IDictionaryConverterInner<,>).MakeGenericType(keyType, valueType),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null);

            return converter;
        }

        /// <inheritdoc />
        private class IDictionaryConverterInner<TKey, TValue> : JsonConverter<Dictionary<TKey, TValue>>
        {
            private readonly JsonConverter<Dictionary<TKey, TValue>> _innerConverter;

            public IDictionaryConverterInner(JsonSerializerOptions options)
            {
                _innerConverter =
                    (JsonConverter<Dictionary<TKey, TValue>>)options.GetConverter(typeof(Dictionary<TKey, TValue>));
            }

            /// <inheritdoc />
            public override Dictionary<TKey, TValue> Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                return _innerConverter.Read(ref reader, typeToConvert, options);
            }

            /// <inheritdoc />
            public override void Write(
                Utf8JsonWriter writer,
                Dictionary<TKey, TValue> value,
                JsonSerializerOptions options)
            {
                _innerConverter.Write(writer, value, options);
            }
        }
    }
}
