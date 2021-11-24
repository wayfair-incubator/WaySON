using FluentAssertions;
using NUnit.Framework;
using System.Text.Json.Serialization;
using WaySON.Serializers;

namespace Wayson.UnitTests.Serialization
{
    [TestFixture]
    public class LongSerializationTests
    {
        private class TestLongObj
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("long")]
            public long Long { get; set; }
        }

        [Test]
        public void Can_Deserialize_NonQuoted_Long()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Long"":1
                    }
                    ";

            // act
            var testLongObj = WaySONSerializer.Deserialize<TestLongObj>(json);

            // assert
            testLongObj.Long.Should().Be(1);
            testLongObj.Name.Should().Be("name1");
        }

        [Test]
        public void Can_Deserialize_Quoted_Long()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Long"":""1""
                    }
                    ";

            // act
            var testLongObj = WaySONSerializer.Deserialize<TestLongObj>(json);

            // assert
            testLongObj.Long.Should().Be(1L);
            testLongObj.Name.Should().Be("name1");
        }
    }
}
