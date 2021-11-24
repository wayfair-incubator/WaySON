using FluentAssertions;
using NUnit.Framework;
using System.Text.Json.Serialization;
using WaySON.Serializers;

namespace WaySON.UnitTests.Serialization
{
    [TestFixture]
    public class BasicSerializationTests
    {
        private class TestObject
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("value")]
            public int Value { get; set; }

            [JsonIgnore]
            public string Ignored { get; set; }
        }

        private const string TestJson = "{\"name\":\"name1\",\"value\":222}";

        private static readonly TestObject TestObj = new TestObject
        {
            Name = "name1",
            Value = 222,
            Ignored = "Ignored9"
        };

        [Test]
        public void Can_Serialize_Correctly()
        {
            // act
            var json = WaySONSerializer.Serialize(TestObj);

            // assert
            json.Should().Be(TestJson);
        }

        [Test]
        public void Can_Deserialize_Correctly()
        {
            // act
            var testObject = WaySONSerializer.Deserialize<TestObject>(TestJson);

            // assert
            testObject.Should().BeEquivalentTo(
                new TestObject
                {
                    Name = TestObj.Name,
                    Value = TestObj.Value
                });
        }
    }
}
