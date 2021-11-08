using FluentAssertions;
using NUnit.Framework;
using System.Text.Json.Serialization;
using Wayfair.Text.Json.Serializers;

namespace Wayfair.Text.Json.UnitTests.Serialization
{
    [TestFixture]
    public class IntSerializationTests
    {
        private class TestIntObj
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("int")]
            public int Int { get; set; }
        }

        [Test]
        public void Can_Deserialize_NonQuoted_Int()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Int"":1
                    }
                    ";

            // act
            var testIntObj = WayfairJsonSerializer.Deserialize<TestIntObj>(json);

            // assert
            testIntObj.Int.Should().Be(1);
            testIntObj.Name.Should().Be("name1");
        }

        [Test]
        public void Can_Deserialize_Quoted_Int()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Int"":""1""
                    }
                    ";

            // act
            var testIntObj = WayfairJsonSerializer.Deserialize<TestIntObj>(json);

            // assert
            testIntObj.Int.Should().Be(1);
            testIntObj.Name.Should().Be("name1");
        }
    }
}
