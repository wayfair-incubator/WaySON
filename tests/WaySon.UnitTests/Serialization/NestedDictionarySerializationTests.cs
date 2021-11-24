using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using WaySON.Serializers;

namespace Wayson.UnitTests.Serialization
{
    [TestFixture]
    public class NestedDictionarySerializationTests
    {
        private const string Json = "{\"0\":{\"10\":1,\"20\":2},\"1\":{\"40\":1,\"50\":2}}";

        private readonly Dictionary<int, Dictionary<int, decimal>> _nestedDictionary =
            new Dictionary<int, Dictionary<int, decimal>>
            {
                { 0, new Dictionary<int, decimal> { { 10, 1 }, { 20, 2 } } },
                { 1, new Dictionary<int, decimal> { { 40, 1 }, { 50, 2 } } }
            };

        [Test]
        public void Can_Serialize_Nested_Dict()
        {
            // act
            var json = WaySONSerializer.Serialize(_nestedDictionary);

            // assert
            json.Should().Be("{\"0\":{\"10\":1,\"20\":2},\"1\":{\"40\":1,\"50\":2}}");
        }

        [Test]
        public void Can_Deserialize_Nested_Dict()
        {
            // act
            var nestedDictionary = WaySONSerializer.Deserialize<Dictionary<int, Dictionary<int, decimal>>>(Json);

            // assert
            nestedDictionary.Should().BeEquivalentTo(_nestedDictionary);
        }
    }
}
