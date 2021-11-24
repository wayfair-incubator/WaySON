using FluentAssertions;
using NUnit.Framework;
using System.Text.Json.Serialization;
using WaySON.Serializers;

namespace Wayson.UnitTests.Serialization
{
    [TestFixture]
    public class DoubleSerializationTests
    {
        private class TestDoubleObj
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("double")]
            public double Double { get; set; }
        }

        [Test]
        public void Can_Deserialize_NonQuoted_Double_NoDecimal()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Double"":1
                    }
                    ";

            // act
            var testDoubleObj = WaySONSerializer.Deserialize<TestDoubleObj>(json);

            // assert
            testDoubleObj.Double.Should().Be(1);
            testDoubleObj.Name.Should().Be("name1");
        }

        [Test]
        public void Can_Deserialize_NonQuoted_Double()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Double"":1.0
                    }
                    ";

            // act
            var testDoubleObj = WaySONSerializer.Deserialize<TestDoubleObj>(json);

            // assert
            testDoubleObj.Double.Should().Be(1.0);
            testDoubleObj.Name.Should().Be("name1");
        }

        [Test]
        public void Can_Deserialize_Quoted_Double()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Double"":""1.0""
                    }
                    ";

            // act
            var testDoubleObj = WaySONSerializer.Deserialize<TestDoubleObj>(json);

            // assert
            testDoubleObj.Double.Should().Be(1.0);
            testDoubleObj.Name.Should().Be("name1");
        }

        [Test]
        public void Does_Serialize_Double_With_Decimals()
        {
            // arrange
            const double testDouble = 888.88888;

            // act
            var serialized = WaySONSerializer.Serialize(testDouble);

            // assert
            serialized.Should().Be("888.88888");
        }

        [Test]
        public void Does_Serialize_Double_With_One_Decimal()
        {
            // arrange
            const double testDouble = 888.8;

            // act
            var serialized = WaySONSerializer.Serialize(testDouble);

            // assert
            serialized.Should().Be("888.8");
        }

        [Test]
        public void Does_Serialize_Double_With_Many_Decimals()
        {
            // arrange
            const double testDouble = .8888888888888888888888888; // 25 significant digits, over the max (16)

            // act
            var serialized = WaySONSerializer.Serialize(testDouble);

            // assert
            serialized.Should().Be("0.8888888888888888", "because it is truncated to the max (16) significant digits");
        }

        [Test]
        public void Does_Serialize_Large_Double_No_Decimals()
        {
            // arrange
            const double testDouble = 888888888888888; // 15 digits

            // act
            var serialized = WaySONSerializer.Serialize(testDouble);

            // assert
            serialized.Should().Be("888888888888888");
        }

        [Test]
        public void Does_Serialize_Double_And_Add_Decimal()
        {
            // arrange
            const double testDouble = 1;

            // act
            var serialized = WaySONSerializer.Serialize(testDouble);

            // assert
            serialized.Should().Be("1");
        }
    }
}
