using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using WaySON.Serializers;

namespace Wayson.UnitTests.Serialization
{
    [TestFixture]
    public class DictionaryLongIntSerializationTests
    {
        private class TestDictLongIntObj
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("dict")]
            public Dictionary<long, int> Dict { get; set; }
        }

        [Test]
        public void Can_Deserialize_DictionaryLongInt_Into_Object_NonQuoted()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Dict"":
                        {
                            ""1"":1,
                            ""2"":2
                        }
                    }
                    ";

            var expectedDictionary = new Dictionary<long, int>
            {
                { 1, 1 },
                { 2, 2 }
            };

            // act
            var testDictLongIntObj = WaySONSerializer.Deserialize<TestDictLongIntObj>(json);

            // assert
            testDictLongIntObj.Name.Should().Be("name1");
            testDictLongIntObj.Dict.Should().BeEquivalentTo(expectedDictionary);
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
                            ""1"":1,
                            ""Hello"":2
                        }
                    }
                    ";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WaySONSerializer.Deserialize<TestDictLongIntObj>(json);
            }
            catch (JsonException)
            {
                exceptionThrown = true;
            }

            // assert
            exceptionThrown.Should().BeTrue("because the JSON contained a bad key for Dictionary<long, int>");
        }

        [Test]
        public void Deserializing_Dictionary_With_No_StartObject_Throws_JsonException()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Dict"":
                            ""1"":1,
                            ""2"":2
                        }
                    }
                    ";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WaySONSerializer.Deserialize<TestDictLongIntObj>(json);
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
                            ""1"":1,
                            ""2"":2
                    }
                    ";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WaySONSerializer.Deserialize<TestDictLongIntObj>(json);
            }
            catch (JsonException)
            {
                exceptionThrown = true;
            }

            // assert
            exceptionThrown.Should().BeTrue("because the dictionary JSON contained no EndObject");
        }

        [Test]
        public void Can_Deserialize_DictionaryLongInt_Into_Object_Quoted()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Dict"":
                        {
                            ""1"":""1"",
                            ""2"":""2""
                        }
                    }
                    ";

            var expectedDictionary = new Dictionary<long, int>
            {
                { 1, 1 },
                { 2, 2 }
            };

            // act
            var testDictLongIntObj = WaySONSerializer.Deserialize<TestDictLongIntObj>(json);

            // assert
            testDictLongIntObj.Name.Should().Be("name1");
            testDictLongIntObj.Dict.Should().BeEquivalentTo(expectedDictionary);
        }

        [Test]
        public void Can_Serialize_Dictionary()
        {
            // arrange
            var dictionary = new Dictionary<long, int>
            {
                { 1, 1 },
                { 2, 2 }
            };

            const string expectedJson = "{\"1\":1,\"2\":2}";

            // act
            var json = WaySONSerializer.Serialize(dictionary);

            // assert
            json.Should().Be(expectedJson);
        }
    }
}
