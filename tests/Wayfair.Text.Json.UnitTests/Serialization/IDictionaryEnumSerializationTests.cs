using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text.Json;
using Wayfair.Text.Json.Serializers;

namespace Wayfair.Text.Json.UnitTests.Serialization
{
    [TestFixture]
    public class IDictionaryEnumSerializationTests
    {
        private enum TestEnum
        {
            One,
            Two
        }

        [Test]
        public void Can_Deserialize_IDictionaryEnumLong()
        {
            // arrange
            const string json = @"{""One"":1}";

            IDictionary<TestEnum, long> expectedDictionary = new Dictionary<TestEnum, long>
            {
                { TestEnum.One, 1 }
            };

            // act
            var dictionary = WayfairJsonSerializer.Deserialize<IDictionary<TestEnum, long>>(json);

            // assert
            dictionary.Should().BeEquivalentTo(expectedDictionary);
        }

        [Test]
        public void Can_Serialize_IDictionaryEnumLong()
        {
            // arrange
            IDictionary<TestEnum, long> dictionary = new Dictionary<TestEnum, long>
            {
                { TestEnum.One, 1 },
                { TestEnum.Two, 2 }
            };

            // act
            var json = WayfairJsonSerializer.Serialize(dictionary);

            // assert
            json.Should().Be("{\"One\":1,\"Two\":2}");
        }

        [Test]
        public void Can_Deserialize_IDictionaryEnumNullableDecimal()
        {
            // arrange
            const string json = @"{""One"":0.99999,""Two"":null}";

            IDictionary<TestEnum, decimal?> expectedDictionary = new Dictionary<TestEnum, decimal?>
            {
                { TestEnum.One, 0.99999m },
                { TestEnum.Two, null }
            };

            // act
            var dictionary = WayfairJsonSerializer.Deserialize<IDictionary<TestEnum, decimal?>>(json);

            // assert
            dictionary.Should().BeEquivalentTo(expectedDictionary);
        }

        [Test]
        public void Deserializing_Dictionary_With_Bad_Key_Throws_JsonException()
        {
            // arrange
            const string json = @"{""Five"":0.99999,""Two"":null}";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WayfairJsonSerializer.Deserialize<IDictionary<TestEnum, decimal?>>(json);
            }
            catch (JsonException)
            {
                exceptionThrown = true;
            }

            // assert
            exceptionThrown.Should().BeTrue("Because Five isn't a valid Enum value as a key");
        }

        [Test]
        public void Deserializing_Dictionary_With_No_StartObject_Throws_JsonException()
        {
            // arrange
            const string json = @"""One"":0.99999,""Two"":null}";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WayfairJsonSerializer.Deserialize<IDictionary<TestEnum, decimal?>>(json);
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
            const string json = @"{""One"":0.99999,""Two"":null";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WayfairJsonSerializer.Deserialize<IDictionary<TestEnum, decimal?>>(json);
            }
            catch (JsonException)
            {
                exceptionThrown = true;
            }

            // assert
            exceptionThrown.Should().BeTrue("because the dictionary JSON contained no EndObject");
        }

        [Test]
        public void Can_Serialize_IDictionaryEnumNullableDecimal()
        {
            // arrange
            IDictionary<TestEnum, decimal?> obj = new Dictionary<TestEnum, decimal?>
            {
                { TestEnum.One, 0.99999m },
                { TestEnum.Two, null }
            };

            // act
            var json = WayfairJsonSerializer.Serialize(obj);

            // assert
            json.Should().Be(@"{""One"":0.99999,""Two"":null}");
        }
    }
}
