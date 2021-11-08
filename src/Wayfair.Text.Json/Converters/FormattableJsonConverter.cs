using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wayfair.Text.Json.Converters
{
    /// <inheritdoc cref="IFormattableJsonConverter"/>
    internal abstract class FormattableJsonConverter<T> : JsonConverter<T>, IFormattableJsonConverter
        where T : IFormattable
    {
        /// <summary>
        ///     Construct the converter with the specified format and format provider.
        /// </summary>
        /// <param name="format">The format to use</param>
        /// <param name="formatProvider">The provider to use to format the value or null to obtain the format information from the current locale setting of the operating system.</param>
        protected FormattableJsonConverter(string format, IFormatProvider formatProvider)
        {
            Format = format;
            FormatProvider = formatProvider;
        }

        /// <summary>
        ///     Construct the converter with no format or format provider.
        ///     When no format is provided, the default format defined for the type of the <see cref="IFormattable"></see> implementation is used.
        ///     When no format provider is provided, the format information from the current locale setting of the operating system is used.
        /// </summary>
        protected FormattableJsonConverter()
            : this(null, null)
        { }

        /// <summary>
        ///     The format to use or null to use the default format defined for the type of the <see cref="IFormattable"></see> being formatted.
        /// </summary>
        protected string Format { get; private set; }

        /// <summary>
        ///     The provider to use to format the value or null to obtain the format information from the current locale setting of the operating system.
        /// </summary>
        protected IFormatProvider FormatProvider { get; private set; }

        /// <inheritdoc/>
        public void SetFormat(string format, IFormatProvider formatProvider)
        {
            Format = format;
            FormatProvider = formatProvider;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format, FormatProvider));
        }
    }
}
