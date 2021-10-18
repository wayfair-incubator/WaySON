using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using NUnit.Framework;
using Wayfair.Text.Json.Serializers;

namespace Wayfair.Text.Json.UnitTests.Serialization
{
    [TestFixture]
    public class IDictionaryIntNullableDecimalSerializationTests
    {
        private class TestIDictIntNullDecimalObj
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("dict")]
            public IDictionary<int, decimal?> Dict { get; set; }
        }

        [Test]
        public void Can_Deserialize_IDictionaryIntNullableDecimal_Into_Object()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Dict"":
                        {
                            ""1"":0.9999999999,
                            ""2"": null,
                            ""3"": ""0.9999999999""
                        }
                    }
                    ";

            decimal? dec = 0.9999999999M;

            IDictionary<int, decimal?> expectedDictionary = new Dictionary<int, decimal?>
            {
                { 1, dec },
                { 2, null },
                { 3, dec },
            };

            // act
            var testIDictIntNullDecimalObj = WayfairJsonSerializer.Deserialize<TestIDictIntNullDecimalObj>(json);

            // assert
            testIDictIntNullDecimalObj.Name.Should().Be("name1");
            testIDictIntNullDecimalObj.Dict.Should().BeEquivalentTo(expectedDictionary);
        }

        [Test]
        public void Deserializing_IDictionaryIntNullableDecimal_With_Invalid_Key_Throws_JsonException()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Dict"":
                        {
                            ""1"":0.9999999999,
                            ""2"": null,
                            ""hello"": ""0.9999999999""
                        }
                    }
                    ";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WayfairJsonSerializer.Deserialize<TestIDictIntNullDecimalObj>(json);
            }
            catch (JsonException)
            {
                exceptionThrown = true;
            }

            // assert
            exceptionThrown.Should().BeTrue("because \"hello\" isn't a valid key");
        }

        [Test]
        public void Deserializing_IDictionaryIntNullableDecimal_With_No_StartObject_Throws_JsonException()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Dict"":
                            ""1"":0.9999999999,
                            ""2"": null,
                            ""3"": ""0.9999999999""
                        }
                    }
                    ";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WayfairJsonSerializer.Deserialize<TestIDictIntNullDecimalObj>(json);
            }
            catch (JsonException)
            {
                exceptionThrown = true;
            }

            // assert
            exceptionThrown.Should().BeTrue("because the dictionary JSON contained no StartObject");
        }

        [Test]
        public void Deserializing_IDictionaryIntNullableDecimal_With_No_EndObject_Throws_JsonException()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""Dict"":
                        {
                            ""1"":0.9999999999,
                            ""2"": null,
                            ""3"": ""0.9999999999""
                    }
                    ";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WayfairJsonSerializer.Deserialize<TestIDictIntNullDecimalObj>(json);
            }
            catch (JsonException)
            {
                exceptionThrown = true;
            }

            // assert
            exceptionThrown.Should().BeTrue("because the dictionary JSON contained no EndObject");
        }

        [Test]
        public void Can_Serialize_IDictionaryIntNullableDecimal()
        {
            // arrange
            IDictionary<int, decimal?> dictionary = new Dictionary<int, decimal?>
            {
                { 1, 0.9m },
                { 2, null }
            };

            // act
            var json = WayfairJsonSerializer.Serialize(dictionary);

            // assert
            json.Should().Be("{\"1\":0.9,\"2\":null}");
        }
    }
}
