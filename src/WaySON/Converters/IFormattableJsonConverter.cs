using System;
using System.Text.Json.Serialization;

namespace WaySON.Converters
{
    /// <summary>
    ///      A custom <see cref="JsonConverter"/> for types that are <see cref="IFormattable"/>, to supply formatting options.
    /// </summary>
    public interface IFormattableJsonConverter
    {
        /// <summary>
        ///     Sets the format and format provider to use on Serialization.
        /// </summary>
        /// <param name="format">The format to use</param>
        /// <param name="formatProvider">The provider to use to format the value or null to obtain the format information from the current locale setting of the operating system.</param>
        void SetFormat(string format, IFormatProvider formatProvider);
    }
}
