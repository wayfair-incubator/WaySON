using System;
using System.Text.Json;

namespace Wayfair.Text.Json.Converters
{
    /// <summary>
    ///     A custom <see cref="FormattableJsonConverter{T}"/> for <see cref="DateTime"/>. DateTime can be in any standard format.
    ///     This will also convert a string in <see cref="DateTimeOffset "/> format to <see cref="DateTime"/>, with the Kind property set as Unspecified.
    /// </summary>
    internal sealed class DateTimeConverter : FormattableJsonConverter<DateTime>
    {
        /// <inheritdoc />
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TryGetDateTime(out var dt))
            {
                return dt;
            }

            if (reader.TryGetDateTimeOffset(out var dto))
            {
                return dto.DateTime;
            }

            var dateAsString = reader.GetString();

            if (DateTime.TryParse(dateAsString, out var dateTime))
            {
                return dateTime;
            }

            if (DateTimeOffset.TryParse(dateAsString, out var offset))
            {
                return offset.DateTime;
            }

            return Convert.ToDateTime(dateAsString, FormatProvider);
        }
    }
}
