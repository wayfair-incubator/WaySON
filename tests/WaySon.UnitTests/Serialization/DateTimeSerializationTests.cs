using FluentAssertions;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Text.Json.Serialization;
using WaySON.Serializers;

namespace Wayson.UnitTests.Serialization
{
    [TestFixture]
    public class DateTimeSerializationTests
    {
        private class TestDateTimeObj
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("date")]
            public DateTime Date { get; set; }
        }

        [SetUp]
        public void SetUp()
        {
            WaySONSerializer.SetFormatOptions(
                typeof(DateTime),
                null,
                CultureInfo.GetCultureInfo("en-us").DateTimeFormat);
        }

        [TearDown]
        public void TearDown()
        {
            WaySONSerializer.SetFormatOptions(
                typeof(DateTime),
                null,
                null);
        }

        [Test]
        public void DateTime_Can_Deserialize_Into_Object()
        {
            // arrange
            const string json = "{\"Name\":\"name1\",\"Date\":\"12/25/2019\"}";

            // act
            var testDateTimeObj = WaySONSerializer.Deserialize<TestDateTimeObj>(json);

            // assert
            testDateTimeObj.Name.Should().Be("name1");
            testDateTimeObj.Date.Day.Should().Be(25);
            testDateTimeObj.Date.Month.Should().Be(12);
            testDateTimeObj.Date.Year.Should().Be(2019);
        }

        [Test]
        public void DateTime_Can_Deserialize_DateTime_From_String_In_Simple_Format()
        {
            // arrange
            const string dateAsString = "\"12/25/2019\"";

            // act
            var dateTime = WaySONSerializer.Deserialize<DateTime>(dateAsString);

            // assert
            dateTime.Day.Should().Be(25);
            dateTime.Month.Should().Be(12);
            dateTime.Year.Should().Be(2019);
        }

        [Test]
        public void Can_Deserialize_DateTime_From_String_In_DateTimeOffset_Format()
        {
            // arrange
            const string dateAsString = "\"2/3/2021 4:05:06 AM -05:00\"";

            // act
            var dateTime = WaySONSerializer.Deserialize<DateTime>(dateAsString);

            // assert
            dateTime.Day.Should().Be(3);
            dateTime.Month.Should().Be(2);
            dateTime.Year.Should().Be(2021);
        }

        [Test]
        public void DateTime_Can_Deserialize_DateTime_From_String_In_ISO_Format()
        {
            // arrange
            const string dateAsString = "\"2019-12-25T01:01:01\"";

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
        public void DateTime_Can_Serialize_Correctly()
        {
            // arrange
            var testDateTimeObj = new TestDateTimeObj
            {
                Name = "name1",
                Date = new DateTime(2019, 12, 25, 0, 0, 0)
            };

            const string expectedJson = "{\"name\":\"name1\",\"date\":\"12/25/2019 12:00:00 AM\"}";

            // act
            var json = WaySONSerializer.Serialize(testDateTimeObj);

            // assert
            json.Should().Be(expectedJson);
        }
    }
}
