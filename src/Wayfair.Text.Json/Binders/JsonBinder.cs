using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Wayfair.Text.Json.Binders
{
    /// <summary>
    ///     Used to Bind JSON to object instances.
    /// </summary>
    public static class JsonBinder
    {
        /// <summary>
        ///     Bind values from a section of JSON to the corresponding fields of an object.
        ///     Fields not present in the JSON will be left alone, preserving their initial value.
        /// </summary>
        /// <param name="instance">The object instance to bind to</param>
        /// <param name="json">The JSON string containing the section of values to bind</param>
        /// <param name="section">The name of the section from the JSON to bind to the object instance</param>
        /// <typeparam name="T">Type of the object to bind to</typeparam>
        /// <returns>The object instance with the section of JSON values bound to their corresponding fields</returns>
        public static T BindToObject<T>(T instance, string json, string section)
        {
            using (var stream = CreateStream(json))
            {
                var config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();

                config.GetSection(section).Bind(instance);
            }

            return instance;
        }

        /// <summary>
        ///     Binds values from JSON to the corresponding fields of an object.
        ///     Fields not present in the JSON will be left alone, preserving their initial value.
        /// </summary>
        /// <param name="instance">The object instance to bind to</param>
        /// <param name="json">The JSON string containing the values to bind</param>
        /// <typeparam name="T">Type of the object to bind to</typeparam>
        /// <returns>The object instance with the JSON values bound to their corresponding fields</returns>
        public static T BindToObject<T>(T instance, string json)
        {
            using (var stream = CreateStream(json))
            {
                var config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();

                config.Bind(instance);
            }

            return instance;
        }

        private static Stream CreateStream(string s)
        {
            var bytes = Encoding.ASCII.GetBytes(s);

            return new MemoryStream(bytes);
        }
    }
}
