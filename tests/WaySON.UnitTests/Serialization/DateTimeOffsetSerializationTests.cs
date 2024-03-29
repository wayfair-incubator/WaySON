﻿using FluentAssertions;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Text.Json.Serialization;
using WaySON.Serializers;

namespace WaySON.UnitTests.Serialization
{
    [TestFixture]
    public class DateTimeOffsetSerializationTests
    {
        [SetUp]
        public void SetUp()
        {
            WaySONSerializer.SetFormatOptions(
                typeof(DateTimeOffset),
                null,
                CultureInfo.GetCultureInfo("en-us").DateTimeFormat);
        }

        [TearDown]
        public void SetupAndTeardown()
        {
            WaySONSerializer.SetFormatOptions(
                typeof(DateTimeOffset),
                null,
                null);
        }

        private class TestDateTimeOffsetObj
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("date")]
            public DateTimeOffset Date { get; set; }
        }

        [Test]
        public void DateTimeOffset_Can_Deserialize_Into_Object()
        {
            // arrange
            const string json = "{\"Name\":\"name1\",\"Date\":\"12/25/2019 11:34:52 PM -05:00\"}";

            // act
            var testDateTimeOffsetObj = WaySONSerializer.Deserialize<TestDateTimeOffsetObj>(json);

            // assert
            testDateTimeOffsetObj.Name.Should().Be("name1");
            testDateTimeOffsetObj.Date.Day.Should().Be(25);
            testDateTimeOffsetObj.Date.Month.Should().Be(12);
            testDateTimeOffsetObj.Date.Year.Should().Be(2019);
            testDateTimeOffsetObj.Date.Offset.Should().Be(TimeSpan.FromHours(-5));
        }

        [Test]
        public void DateTimeOffset_Can_Deserialize_DateTimeOffset_From_String_In_Simple_Format()
        {
            // arrange
            const string dateAsString = "\"12/25/2019\"";

            // act
            var dateTimeOffset = WaySONSerializer.Deserialize<DateTimeOffset>(dateAsString);

            // assert
            dateTimeOffset.Day.Should().Be(25);
            dateTimeOffset.Month.Should().Be(12);
            dateTimeOffset.Year.Should().Be(2019);
        }

        [Test]
        public void DateTime_Can_Deserialize_DateTimeOffset_From_String_In_ISO_Format()
        {
            // arrange
            const string dateAsString = "\"2019-12-25T01:01:01Z\"";

            // act
            var dateTime = WaySONSerializer.Deserialize<DateTime>(dateAsString);

            // assert
            dateTime.Day.Should().Be(25);
            dateTime.Month.Should().Be(12);
            dateTime.Year.Should().Be(2019);
            dateTime.Hour.Should().Be(1);
            dateTime.Minute.Should().Be(1);
            dateTime.Second.Should().Be(1);
        }

        [Test]
        public void DateTimeOffset_Can_Serialize_Correctly()
        {
            // arrange
            var testDateTimeOffsetObj = new TestDateTimeOffsetObj
            {
                Name = "name1",
                Date = new DateTimeOffset(new DateTime(2019, 12, 25, 12, 0, 0), TimeSpan.FromHours(-5))
            };

            const string expectedJson = "{\"name\":\"name1\",\"date\":\"12/25/2019 12:00:00 PM -05:00\"}";

            // act
            var json = WaySONSerializer.Serialize(testDateTimeOffsetObj);

            // assert
            json.Should().Be(expectedJson);
        }
    }
}
