using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wayfair.Text.Json.Converters;

namespace Wayfair.Text.Json.Serializers
{
    /// <summary>
    ///     Used to serialize and deserialize JSON.
    ///     Builds on <see cref="System.Text.Json "/> by adding custom <see cref="JsonConverter"/>s.
    ///     Singleton.
    /// </summary>
    public sealed class WayfairJsonSerializer
    {
        private static readonly Lazy<WayfairJsonSerializer> LazyInstance = new Lazy<WayfairJsonSerializer>(
            () => new WayfairJsonSerializer()
        );

        private JsonSerializerOptions _options;

        private WayfairJsonSerializer()
        {
            _options = CreateSerializerOptions();
        }

        /// <summary>
        ///     Warning! This will change the options for the singleton, so be careful of context switching.
        ///     Sets the <see cref="JsonSerializerOptions"/>, including the <see cref="JsonConverter"/>s.
        ///     Use this if you want settings other than the default.
        /// </summary>
        /// <param name="options">The <see cref="JsonSerializerOptions"/>, including the <see cref="JsonConverter"/>s</param>
        public static void SetSerializerOptions(JsonSerializerOptions options)
        {
            Serializer()._options = options;
        }

        /// <summary>
        ///     Warning! This will change the options for the singleton, so be careful of context switching.
        ///     Sets the format options for serializing output.
        /// </summary>
        /// <param name="type">The type to set the format options for</param>
        /// <param name="format">The format to use</param>
        /// <param name="formatProvider">The provider to use to format the value or null to obtain the format information from the current locale setting of the operating system.</param>
        public static void SetFormatOptions(Type type, string format, IFormatProvider formatProvider)
        {
            foreach (var converter in Serializer()._options.Converters)
            {
                Type[] generic = null;

                try
                {
                    generic = converter.GetType().BaseType?.GetGenericArguments();
                }
                catch (NotSupportedException)
                {
                    // GetGenericArguments will throw a NotSupportedException if there are no generic arguments, so we skip those with none.
                }

                if (generic == null || !generic.Any() || type != generic[0])
                {
                    continue;
                }

                if (converter is IFormattableJsonConverter jsonConverter)
                {
                    jsonConverter.SetFormat(format, formatProvider);
                }
            }
        }

        /// <summary>
        ///     Warning! This will change the options for the singleton, so be careful of context switching.
        ///     Sets the format options for serializing output.
        /// </summary>
        /// <param name="formatOptions">An <see cref="IDictionary{TKey,TValue}"/> of <see cref="Type"/> to <see cref="Tuple"/> of format string and <see cref="IFormatProvider"/></param>
        public static void SetFormatOptions(IDictionary<Type, Tuple<string, IFormatProvider>> formatOptions)
        {
            foreach (var converter in Serializer()._options.Converters)
            {
                try
                {
                    if (converter is IFormattableJsonConverter jsonConverter)
                    {
                        var generic = converter.GetType().BaseType?.GetGenericArguments();

                        if (generic != null &&
                            formatOptions != null &&
                            formatOptions.TryGetValue(generic[0], out var tuple))
                        {
                            jsonConverter.SetFormat(tuple.Item1, tuple.Item2);
                        }
                    }
                }
                catch (NotSupportedException)
                {
                    // GetGenericArguments will throw a NotSupportedException if there are no generic arguments, so we skip those with none.
                }
            }
        }

        /// <summary>
        ///     Serialize a value into a JSON string.
        /// </summary>
        /// <param name="value">The value to serialize from</param>
        /// <typeparam name="T">The type of the value being serialized from</typeparam>
        /// <returns>JSON string</returns>
        public static string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value, Serializer()._options);
        }

        /// <summary>
        ///     Deserialize a JSON string into a value of type T.
        /// </summary>
        /// <param name="value">JSON string</param>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <returns>Object of type T</returns>
        public static T Deserialize<T>(string value)
        {
            return JsonSerializer.Deserialize<T>(value, Serializer()._options);
        }

        /// <summary>
        ///     Deserialize a JSON string into a value of a given type.
        ///     Use this if you do not know the type at compile time.
        /// </summary>
        /// <param name="value">JSON string</param>
        /// <param name="type">The type to deserialize into</param>
        /// <returns>Object of type "type"</returns>
        public static object Deserialize<T>(string value, Type type)
        {
            return JsonSerializer.Deserialize(value, type, Serializer()._options);
        }

        /// <summary>
        ///     The options that the serializer uses.
        /// </summary>
        /// <returns>The <see cref="JsonSerializerOptions"/>, including the <see cref="JsonConverter"/>s</returns>
        public static JsonSerializerOptions Options()
        {
            return Serializer()._options;
        }

        /// <summary>
        ///     The default additional custom <see cref="JsonConverter"/>s that the serializer uses.
        /// </summary>
        /// <returns>The converters used</returns>
        private static IEnumerable<JsonConverter> DefaultJsonConverters()
        {
            var converters = new List<JsonConverter>
            {
                new StringConverter(),
                new Int32Converter(),
                new DoubleConverter(),
                new LongConverter(),
                new DecimalConverter(),
                new DateTimeConverter(),
                new DateTimeOffsetConverter(),
                new EnumConverter(),
                new DictionaryIntStringConverter(),
                new DictionaryIntConverter(),
                new DictionaryLongConverter(),
                new DictionaryLongStringConverter(),
                new DictionaryEnumConverter(),
                new DictionaryStringIntConverter(),
                new DictionaryStringLongConverter(),
                new DictionaryConverter(),
                new IDictionaryConverter(),
            };

            return converters;
        }

        /// <summary>
        ///     The default base serializer options.
        /// </summary>
        /// <returns>JsonSerializerOptions reflecting the default options (without converters)</returns>
        private static JsonSerializerOptions DefaultJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null,
            };
        }

        private static JsonSerializerOptions CreateSerializerOptions()
        {
            var options = DefaultJsonSerializerOptions();

            foreach (var jsonConverter in DefaultJsonConverters())
            {
                options.Converters.Add(jsonConverter);
            }

            return options;
        }

        private static WayfairJsonSerializer Serializer()
        {
            return LazyInstance.Value;
        }
    }
}
