using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WaySON.Serializers;

namespace WaySON.UnitTests.Serialization
{
    [TestFixture]
    public class DictionaryIntNullableDecimalSerializationTests
    {
        private class TestDictIntNullDecimalObj
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("dict")]
            public Dictionary<int, decimal?> Dict { get; set; }
        }

        [Test]
        public void Can_Deserialize_DictionaryIntNullableDecimal_Into_Object()
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

            var expectedDictionary = new Dictionary<int, decimal?>
            {
                { 1, dec },
                { 2, null },
                { 3, dec },
            };

            // act
            var testDictIntNullDecimalObj = WaySONSerializer.Deserialize<TestDictIntNullDecimalObj>(json);

            // assert
            testDictIntNullDecimalObj.Name.Should().Be("name1");
            testDictIntNullDecimalObj.Dict.Should().BeEquivalentTo(expectedDictionary);
        }

        [Test]
        public void Can_Serialize_DictionaryIntNullableDecimal()
        {
            // arrange
            var dictionary = new Dictionary<int, decimal?>
            {
                { 1, 0.9m },
                { 2, null }
            };

            // act
            var json = WaySONSerializer.Serialize(dictionary);

            // assert
            json.Should().Be("{\"1\":0.9,\"2\":null}");
        }
    }
}
