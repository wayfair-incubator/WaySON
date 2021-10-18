using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;
using Wayfair.Text.Json.Serializers;

namespace Wayfair.Text.Json.UnitTests.Serialization
{
    [TestFixture]
    public class DictionaryEnumSerializationTests
    {
        private enum TestEnum
        {
            One,
            Two
        }

        [Test]
        public void Can_Deserialize_Enum_Into_Its_Name()
        {
            // arrange
            const string json = "\"One\"";

            // act
            var testEnum = WayfairJsonSerializer.Deserialize<TestEnum>(json);

            // assert
            testEnum.Should().Be(TestEnum.One);
        }

        [Test]
        public void Can_Deserialize_DictionaryEnumLong()
        {
            // arrange
            const string json = @"{""One"":1}";

            var expectedDictionary = new Dictionary<TestEnum, long>
            {
                { TestEnum.One, 1 }
            };

            // act
            var dictionary = WayfairJsonSerializer.Deserialize<Dictionary<TestEnum, long>>(json);

            // assert
            dictionary.Should().BeEquivalentTo(expectedDictionary);
        }

        [Test]
        public void Can_Serialize_DictionaryEnumLong()
        {
            // arrange
            var dictionary = new Dictionary<TestEnum, long>
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
        public void Can_Deserialize_DictionaryEnumNullableDecimal()
        {
            // arrange
            const string json = @"{""One"":0.99999,""Two"":null}";

            var expectedDictionary = new Dictionary<TestEnum, decimal?>
            {
                { TestEnum.One, 0.99999m },
                { TestEnum.Two, null }
            };

            // act
            var obj = WayfairJsonSerializer.Deserialize<Dictionary<TestEnum, decimal?>>(json);

            // assert
            obj.Should().BeEquivalentTo(expectedDictionary);
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
                var _ = WayfairJsonSerializer.Deserialize<Dictionary<TestEnum, decimal?>>(json);
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
                var _ = WayfairJsonSerializer.Deserialize<Dictionary<TestEnum, decimal?>>(json);
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
                var _ = WayfairJsonSerializer.Deserialize<Dictionary<TestEnum, decimal?>>(json);
            }
            catch (JsonException)
            {
                exceptionThrown = true;
            }

            // assert
            exceptionThrown.Should().BeTrue("because the dictionary JSON contained no EndObject");
        }

        [Test]
        public void Can_Serialize_DictionaryEnumNullableDecimal()
        {
            // arrange
            var obj = new Dictionary<TestEnum, decimal?>
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
