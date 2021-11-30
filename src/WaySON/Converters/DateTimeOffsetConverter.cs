using System;
using System.Text.Json;

namespace WaySON.Converters
{
    /// <summary>
    ///     A custom <see cref="FormattableJsonConverter{T}"/> for <see cref="DateTimeOffset"/>. DateTimeOffset can be in any standard format.
    /// </summary>
    internal sealed class DateTimeOffsetConverter : FormattableJsonConverter<DateTimeOffset>
    {
        /// <inheritdoc />
        public override DateTimeOffset Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (FormatProvider is null || Format is null)
            {
                return reader.TryGetDateTimeOffset(out var dto)
                    ? dto
                    : DateTimeOffset.Parse(reader.GetString(), FormatProvider);
            }

            return DateTimeOffset.ParseExact(reader.GetString(), Format, FormatProvider);
        }
    }
}
