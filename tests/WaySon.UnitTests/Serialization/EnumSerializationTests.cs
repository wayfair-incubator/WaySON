using FluentAssertions;
using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;
using WaySON.Serializers;

namespace Wayson.UnitTests.Serialization
{
    [TestFixture]
    public class EnumSerializationTests
    {
        private enum TestEnum
        {
            One = 1,
            Two = 2
        }

        private class TestEnumObj
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("testEnum")]
            public TestEnum TestEnum { get; set; }
        }

        [Test]
        public void Can_Deserialize_From_Number()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""TestEnum"":1
                    }
                    ";

            // act
            var testEnumObj = WaySONSerializer.Deserialize<TestEnumObj>(json);

            // assert
            testEnumObj.TestEnum.Should().Be(TestEnum.One);
            testEnumObj.Name.Should().Be("name1");
        }

        [Test]
        public void Can_Deserialize_From_Quoted_Number()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""TestEnum"":""1""
                    }
                    ";

            // act
            var testEnumObj = WaySONSerializer.Deserialize<TestEnumObj>(json);

            // assert
            testEnumObj.TestEnum.Should().Be(TestEnum.One);
            testEnumObj.Name.Should().Be("name1");
        }

        [Test]
        public void Can_Deserialize_From_Enum_Name()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""TestEnum"":""Two""
                    }
                    ";

            // act
            var testEnumObj = WaySONSerializer.Deserialize<TestEnumObj>(json);

            // assert
            testEnumObj.TestEnum.Should().Be(TestEnum.Two);
            testEnumObj.Name.Should().Be("name1");
        }

        [Test]
        public void Deserializing_From_Invalid_Enum_Name_Throws_JsonException()
        {
            // arrange
            const string json = @"
                    {
                        ""Name"":""name1"",
                        ""TestEnum"":""Five""
                    }
                    ";

            var exceptionThrown = false;

            // act
            try
            {
                var _ = WaySONSerializer.Deserialize<TestEnumObj>(json);
            }
            catch (JsonException)
            {
                exceptionThrown = true;
            }

            // assert
            exceptionThrown.Should().BeTrue("because an invalid enum string was passed");
        }
    }
}
