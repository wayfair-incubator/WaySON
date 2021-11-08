using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wayfair.Text.Json.Serializers;

namespace Wayfair.Text.Json.UnitTests.Serialization
{
    [TestFixture]
    public class DictionaryIntStringSerializationTests
    {
        private class TestDictIntStringObj
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("dict")]
            public Dictionary<int, string> Dict { get; set; }
        }

        [Test]
        public void Can_Deserialize_DictionaryIntString_Into_Object()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Dict"":
                        {
                            ""1"":""one"",
                            ""2"":""two""
                        }
                    }
                    ";

            var expectedDictionary = new Dictionary<int, string>
            {
                { 1, "one" },
                { 2, "two" }
            };

            // act
            var testDictIntStringObj = WayfairJsonSerializer.Deserialize<TestDictIntStringObj>(json);

            // assert
            testDictIntStringObj.Name.Should().Be("name1");
            testDictIntStringObj.Dict.Should().BeEquivalentTo(expectedDictionary);
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
                            ""1"":""one"",
                            ""Hello"":""two""
                        }
                    }
                    ";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WayfairJsonSerializer.Deserialize<TestDictIntStringObj>(json);
            }
            catch (JsonException)
            {
                exceptionThrown = true;
            }

            // assert
            exceptionThrown.Should().BeTrue("because the JSON contained a bad key for Dictionary<int, string>");
        }

        [Test]
        public void Deserializing_Dictionary_With_No_StartObject_Throws_JsonException()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Dict"":
                            ""1"":""one"",
                            ""Hello"":""two""
                        }
                    }
                    ";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WayfairJsonSerializer.Deserialize<TestDictIntStringObj>(json);
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
                            ""1"":""one"",
                            ""2"":""two""
                    }
                    ";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WayfairJsonSerializer.Deserialize<TestDictIntStringObj>(json);
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
            var dictionary = new Dictionary<int, string>
            {
                { 1, "one" },
                { 2, "two" }
            };

            const string expectedJson = "{\"1\":\"one\",\"2\":\"two\"}";

            // act
            var json = WayfairJsonSerializer.Serialize(dictionary);

            // assert
            json.Should().Be(expectedJson);
        }
    }
}
