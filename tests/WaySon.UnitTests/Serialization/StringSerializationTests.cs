using FluentAssertions;
using NUnit.Framework;
using System.Text.Json.Serialization;
using WaySON.Serializers;

namespace WaySON.UnitTests.Serialization
{
    [TestFixture]
    public class StringSerializationTests
    {
        private class TestStringObj
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        [Test]
        public void Can_Deserialize_Standard_String()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1""
                    }
                    ";

            // act
            var testStringObj = WaySONSerializer.Deserialize<TestStringObj>(json);

            // assert
            testStringObj.Name.Should().Be("name1");
        }

        [Test]
        public void Can_Deserialize_Integer_String()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":1
                    }
                    ";

            // act
            var testStringObj = WaySONSerializer.Deserialize<TestStringObj>(json);

            // assert
            testStringObj.Name.Should().Be("1");
        }

        [Test]
        public void Can_Deserialize_Double_String()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":1.23456789
                    }
                    ";

            // act
            var testStringObj = WaySONSerializer.Deserialize<TestStringObj>(json);

            // assert
            testStringObj.Name.Should().Be("1.23456789");
        }
    }
}
