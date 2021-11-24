using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using WaySON.Serializers;

namespace Wayson.UnitTests.Serialization
{
    [TestFixture]
    public class WaySONSerializerOptionsTests
    {
        [TearDown]
        public void ResetFormatOptions()
        {
            // Need to reset options to default so that other unit tests will pass (because they use the same singleton)
            WaySONSerializer.SetFormatOptions(typeof(DateTimeOffset), null, null);
        }

        [Test]
        public void Does_Serialize_Using_Format_String_DateTimeOffset()
        {
            // arrange
            const string formatString = "s";
            var formatProvider = CultureInfo.CurrentCulture.DateTimeFormat;
            WaySONSerializer.SetFormatOptions(typeof(DateTimeOffset), formatString, formatProvider);

            var dateTimeOffset = new DateTimeOffset(2020, 12, 1, 2, 3, 4, TimeSpan.FromHours(5));

            // act
            var serializedDateTimeOffset = WaySONSerializer.Serialize(dateTimeOffset);

            // assert
            serializedDateTimeOffset.Should().Be("\"2020-12-01T02:03:04\"");
        }

        [Test]
        public void Does_Serialize_Using_Format_String_And_Provider_DateTimeOffset()
        {
            // arrange
            const string formatString = "s";
            var formatProvider = CultureInfo.GetCultureInfo("fr-FR").DateTimeFormat;
            WaySONSerializer.SetFormatOptions(typeof(DateTimeOffset), formatString, formatProvider);

            var dateTimeOffset = new DateTimeOffset(2020, 12, 1, 2, 3, 4, TimeSpan.FromHours(5));

            // act
            var serializedDateTimeOffset = WaySONSerializer.Serialize(dateTimeOffset);

            // assert
            serializedDateTimeOffset.Should().Be("\"2020-12-01T02:03:04\"");
        }

        [Test]
        public void Does_Serialize_Using_Format_String_And_Provider_From_Dictionary_DateTimeOffset()
        {
            // arrange
            const string formatString = "u";
            var formatProvider = CultureInfo.GetCultureInfo("fr-FR").DateTimeFormat;

            var dictionary = new Dictionary<Type, Tuple<string, IFormatProvider>>
            {
                { typeof(DateTimeOffset), new Tuple<string, IFormatProvider>(formatString, formatProvider) }
            };

            WaySONSerializer.SetFormatOptions(dictionary);

            var dateTimeOffset = new DateTimeOffset(2020, 12, 1, 2, 3, 4, TimeSpan.FromHours(5));

            // act
            var serializedDateTimeOffset = WaySONSerializer.Serialize(dateTimeOffset);

            // assert
            serializedDateTimeOffset.Should().Be("\"2020-11-30 21:03:04Z\"");
        }

        [Test]
        public void Does_DeSerialize_Using_Format_String_And_Provider_From_Dictionary_DateTimeOffset()
        {
            // arrange
            var formatProvider = CultureInfo.GetCultureInfo("en-US").DateTimeFormat;
            const string format = @"MM/dd/yyyy H:mm zzz";
            WaySONSerializer.SetFormatOptions(typeof(DateTimeOffset), format, formatProvider);

            const string dateAsString = "\"12/08/2007 6:54 -6:00\"";

            // act
            var dateTimeOffset = WaySONSerializer.Deserialize<DateTimeOffset>(dateAsString);

            // assert
            dateTimeOffset.Day.Should().Be(08);
            dateTimeOffset.Month.Should().Be(12);
            dateTimeOffset.Year.Should().Be(2007);
            dateTimeOffset.Hour.Should().Be(6);
            dateTimeOffset.Minute.Should().Be(54);
            dateTimeOffset.Second.Should().Be(0);
        }

        [Test]
        public void Deserialize_Throws_When_Format_Given_with_no_offset_DateTimeOffset()
        {
            // arrange
            var formatProvider = CultureInfo.GetCultureInfo("en-US").DateTimeFormat;
            const string format = @"MM/dd/yyyy H:mm zzz";
            WaySONSerializer.SetFormatOptions(typeof(DateTimeOffset), format, formatProvider);

            const string dateAsString = "\"12/08/2007 6:54 \"";

            // act + assert
            Assert.Throws(
                typeof(FormatException),
                () => WaySONSerializer.Deserialize<DateTimeOffset>(dateAsString));
        }

        [Test]
        public void Deserialize_Throws_When_Format_Given_with_wrong_Format_DateTimeOffset()
        {
            // arrange
            var formatProvider = CultureInfo.GetCultureInfo("en-US").DateTimeFormat;
            const string format = "u";
            WaySONSerializer.SetFormatOptions(typeof(DateTimeOffset), format, formatProvider);

            const string dateAsString = "\"12/08/2007 6:54 -6:00\"";

            // act + assert
            Assert.Throws(
                typeof(FormatException),
                () => WaySONSerializer.Deserialize<DateTimeOffset>(dateAsString));
        }
    }
}
