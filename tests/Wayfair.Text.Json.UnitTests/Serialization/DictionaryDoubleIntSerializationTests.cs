using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using NUnit.Framework;
using Wayfair.Text.Json.Serializers;

namespace Wayfair.Text.Json.UnitTests.Serialization
{
    [TestFixture]
    public class DictionaryDoubleIntSerializationTests
    {
        private class TestDictDoubleIntObj
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("dict")]
            public Dictionary<double, int> Dict { get; set; }
        }

        [Test]
        public void Can_Deserialize_DictionaryIntString_Into_Object_NonQuoted()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Dict"":
                        {
                            ""1.1"":1,
                            ""2.2"":2
                        }
                    }
                    ";

            var expectedDictionary = new Dictionary<double, int>
            {
                { 1.1, 1 },
                { 2.2, 2 }
            };

            // act
            var testDictDoubleIntObj = WayfairJsonSerializer.Deserialize<TestDictDoubleIntObj>(json);

            // assert
            testDictDoubleIntObj.Name.Should().Be("name1");
            testDictDoubleIntObj.Dict.Should().BeEquivalentTo(expectedDictionary);
        }

        [Test]
        public void Can_Deserialize_DictionaryIntString_Into_Object_Quoted()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Dict"":
                        {
                            ""1.1"":""1"",
                            ""2.2"":""2""
                        }
                    }
                    ";

            var expectedDictionary = new Dictionary<double, int>
            {
                { 1.1, 1 },
                { 2.2, 2 }
            };

            // act
            var testDictDoubleIntObj = WayfairJsonSerializer.Deserialize<TestDictDoubleIntObj>(json);

            // assert
            testDictDoubleIntObj.Name.Should().Be("name1");
            testDictDoubleIntObj.Dict.Should().BeEquivalentTo(expectedDictionary);
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
                            ""1.1"":1,
                            ""Hello"":2
                        }
                    }
                    ";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WayfairJsonSerializer.Deserialize<TestDictDoubleIntObj>(json);
            }
            catch (JsonException)
            {
                exceptionThrown = true;
            }

            // assert
            exceptionThrown.Should().BeTrue("because the JSON contained a bad key for Dictionary<double, int>");
        }

        [Test]
        public void Deserializing_Dictionary_With_No_StartObject_Throws_JsonException()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Dict"":
                            ""1.1"":1,
                            ""2.2"":2
                        }
                    }
                    ";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WayfairJsonSerializer.Deserialize<TestDictDoubleIntObj>(json);
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
                            ""1.1"":1,
                            ""2.2"":2
                    }
                    ";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WayfairJsonSerializer.Deserialize<TestDictDoubleIntObj>(json);
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
            var dictionary = new Dictionary<double, int>
            {
                { 1.1, 1 },
                { 2.2, 2 }
            };

            const string expectedJson = "{\"1.1\":1,\"2.2\":2}";

            // act
            var json = WayfairJsonSerializer.Serialize(dictionary);

            // assert
            json.Should().Be(expectedJson);
        }
    }
}
