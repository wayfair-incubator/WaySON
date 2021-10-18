using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using NUnit.Framework;
using Wayfair.Text.Json.Serializers;

namespace Wayfair.Text.Json.UnitTests.Serialization
{
    [TestFixture]
    public class DecimalSerializationTests
    {
        private class TestNullableDecimalObj
        {
            [JsonPropertyName("decimal")]
            public decimal? Decimal { get; set; }
        }

        [Test]
        public void Can_Deserialize_Decimal_Into_Nullable_Decimal_Object()
        {
            const string json = @"
                {
                    ""Decimal"":0.123456789
                }
            ";

            decimal? expectedDecimal = 0.123456789M;

            // act
            var testNullableDecimalObj = WayfairJsonSerializer.Deserialize<TestNullableDecimalObj>(json);

            // assert
            testNullableDecimalObj.Decimal.Should().Be(expectedDecimal);
        }

        [Test]
        public void Can_Deserialize_String_Decimal_Into_Nullable_Decimal_Object()
        {
            const string json = @"
                {
                    ""Decimal"":""0.123456789""
                }
            ";

            decimal? expectedDecimal = 0.123456789M;

            // act
            var testNullableDecimalObj = WayfairJsonSerializer.Deserialize<TestNullableDecimalObj>(json);

            // assert
            testNullableDecimalObj.Decimal.Should().Be(expectedDecimal);
        }

        [Test]
        public void Can_Deserialize_Null_Into_Nullable_Decimal_Object()
        {
            const string json = @"
                {
                    ""Decimal"":null
                }
            ";

            // act
            var testNullableDecimalObj = WayfairJsonSerializer.Deserialize<TestNullableDecimalObj>(json);

            // assert
            testNullableDecimalObj.Decimal.Should().BeNull();
        }

        [Test]
        public void Deserializing_Non_Decimal_Non_String_Throws_JsonException()
        {
            const string json = @"
                {
                    ""Decimal"":1A2B3C4D5E6F7G8H9I
                }
            ";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WayfairJsonSerializer.Deserialize<TestNullableDecimalObj>(json);
            }
            catch (JsonException)
            {
                exceptionThrown = true;
            }

            // assert
            exceptionThrown.Should().BeTrue("because 1A2B3C4D5E6F7G8H9I is an invalid value to deserialize from");
        }
    }
}
