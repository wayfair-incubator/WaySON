using FluentAssertions;
using NUnit.Framework;
using Wayfair.Text.Json.Serializers;

namespace Wayfair.Text.Json.UnitTests.Serialization
{
    [TestFixture]
    public class NullableDecimalTests
    {
        [Test]
        public void Can_Deserialize_Nullable_Decimal_From_Null()
        {
            // arrange
            const string nullString = "null";

            // act
            var dec = WayfairJsonSerializer.Deserialize<decimal?>(nullString);

            // assert
            dec.Should().BeNull();
        }

        [Test]
        public void Can_Serialize_To_Nullable_Decimal_From_Null()
        {
            // arrange
            decimal? dec = null;

            // act
            var json = WayfairJsonSerializer.Serialize(dec);

            // assert
            json.Should().Be("null");
        }

        [Test]
        public void Can_Deserialize_Nullable_Decimal_From_Decimal()
        {
            // arrange
            const string decimalAsString = "0.999999";

            // act
            var dec = WayfairJsonSerializer.Deserialize<decimal?>(decimalAsString);

            // assert
            dec.Should().Be(0.999999M);
        }

        [Test]
        public void Can_Serialize_To_Nullable_Decimal_From_Decimal()
        {
            // arrange
            decimal? dec = 0.999999M;

            // act
            var serialized = WayfairJsonSerializer.Serialize(dec);

            // assert
            serialized.Should().Be("0.999999");
        }
    }
}
