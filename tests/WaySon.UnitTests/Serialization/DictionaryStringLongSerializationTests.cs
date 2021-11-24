using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using WaySON.Serializers;

namespace WaySON.UnitTests.Serialization
{
    [TestFixture]
    public class DictionaryStringLongSerializationTests
    {
        private class TestDictStringLongObj
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("dict")]
            public Dictionary<string, long> Dict { get; set; }
        }

        [Test]
        public void Can_Deserialize_DictionaryStringLong_Into_Object()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Dict"":
                        {
                            ""one"":""1"",
                            ""two"":""2""
                        }
                    }
                    ";

            var expectedDictionary = new Dictionary<string, long>
            {
                { "one", 1 },
                { "two", 2 }
            };

            // act
            var testDictStringLongObj = WaySONSerializer.Deserialize<TestDictStringLongObj>(json);

            // assert
            testDictStringLongObj.Name.Should().Be("name1");
            testDictStringLongObj.Dict.Should().BeEquivalentTo(expectedDictionary);
        }

        [Test]
        public void Deserializing_Dictionary_With_Bad_Key_Throws_JsonException()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Dict"":
                        {
                            ""one"":1,
                            -12345:2
                        }
                    }
                    ";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WaySONSerializer.Deserialize<TestDictStringLongObj>(json);
            }
            catch (JsonException)
            {
                exceptionThrown = true;
            }

            // assert
            exceptionThrown.Should().BeTrue("because the JSON contained a bad key for Dictionary<string, long>");
        }

        [Test]
        public void Deserializing_Dictionary_With_No_StartObject_Throws_JsonException()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Dict"":
                            ""one"":1,
                            ""two"":2
                        }
                    }
                    ";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WaySONSerializer.Deserialize<TestDictStringLongObj>(json);
            }
            catch (JsonException)
            {
                exceptionThrown = true;
            }

            // assert
            exceptionThrown.Should().BeTrue("because the dictionary JSON contained no StartObject");
        }

        [Test]
        public void Deserializing_Dictionary_With_No_EndObject_Throws_JsonException()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Dict"":
                        {
                            ""one"":1,
                            ""two"":2
                    }
                    ";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WaySONSerializer.Deserialize<TestDictStringLongObj>(json);
            }
            catch (JsonException)
            {
                exceptionThrown = true;
            }

            // assert
            exceptionThrown.Should().BeTrue("because the dictionary JSON contained no EndObject");
        }

        [Test]
        public void Can_Serialize_Dictionary()
        {
            // arrange
            var dictionary = new Dictionary<string, long>()
            {
                { "one", 1 },
                { "two", 2 }
            };

            const string expectedJson = "{\"one\":1,\"two\":2}";

            // act
            var json = WaySONSerializer.Serialize(dictionary);

            // assert
            json.Should().Be(expectedJson);
        }
    }
}
