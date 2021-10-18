using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Wayfair.Text.Json.Binders;

namespace Wayfair.Text.Json.UnitTests.Binders
{
    [TestFixture]
    public class JsonBinderTests
    {
        private class TestConfig
        {
            public int One { get; set; }
            public int Two { get; set; }
            public int Three { get; set; }
            public string Four { get; set; }
        }

        [Test]
        public void Can_Bind_All_Fields()
        {
            // arrange
            const string json = @"
                {
                    ""testconfig"": {
                        ""One"":1,
                        ""Two"":1,
                        ""Three"":1,
                        ""Four"":""hello""
                    }
                }";

            var testConfig = new TestConfig
            {
                One = 999,
                Two = 999,
                Three = 999
            };

            // act
            testConfig = JsonBinder.BindToObject(testConfig, json, "testconfig");

            // assert
            testConfig.One.Should().Be(1);
            testConfig.Two.Should().Be(1);
            testConfig.Three.Should().Be(1);
            testConfig.Four.Should().Be("hello");
        }

        [Test]
        public void Can_Bind_Only_Specified_Fields_Leaving_Others_Untouched()
        {
            // arrange
            const string json = @"
                {
                    ""testconfig"": {
                        ""One"":1
                    }
                }";

            var testConfig = new TestConfig
            {
                One = 999,
                Two = 999,
                Three = 999,
                Four = null
            };

            // act
            testConfig = JsonBinder.BindToObject(testConfig, json, "testconfig");

            // assert
            testConfig.One.Should().Be(1);
            testConfig.Two.Should().Be(999);
            testConfig.Three.Should().Be(999);
            testConfig.Four.Should().BeNullOrEmpty();
        }

        [Test]
        public void Should_Not_Bind_Or_Error_When_Section_Does_Not_Exist()
        {
            // arrange
            const string json = @"
                {
                    ""testconfig"": {
                        ""One"":1,
                        ""Two"":1,
                        ""Three"":1,
                        ""Four"":""hello""
                    }
                }";

            var testConfig = new TestConfig();

            // act
            testConfig = JsonBinder.BindToObject(testConfig, json, "NOT_HERE");

            // assert
            testConfig.One.Should().Be(default);
            testConfig.Two.Should().Be(default);
            testConfig.Three.Should().Be(default);
            testConfig.Four.Should().BeNullOrEmpty();
        }

        private class OuterTestConfig
        {
            public int One { get; set; }
            public TestConfig Config { get; set; }
            public Dictionary<string, string> Dictionary { get; set; }

            public OuterTestConfig()
            {
                One = default;
                Config = null;
            }
        }

        [Test]
        public void Should_Bind_All_With_No_Specified_Section()
        {
            // arrange
            const string json = @"
                {
                    ""Config"": {
                        ""One"":1,
                        ""Two"":1,
                        ""Three"":1,
                        ""Four"":""hello""
                    },
                    ""One"": 10
                }";

            var outer = new OuterTestConfig();

            // act
            outer = JsonBinder.BindToObject(outer, json);

            // assert
            outer.One.Should().Be(10);
            outer.Config.One.Should().Be(1);
            outer.Config.Two.Should().Be(1);
            outer.Config.Three.Should().Be(1);
            outer.Config.Four.Should().Be("hello");
        }

        [Test]
        public void Should_Bind_All_With_No_Specified_Section_With_Null_Starting_Field()
        {
            // arrange
            const string json = @"
                {
                    ""Config"": {
                        ""One"":1,
                        ""Two"":1,
                        ""Three"":1,
                        ""Four"":""hello""
                    },
                    ""One"": 10
                }";

            var outer = new OuterTestConfig
            {
                Config = null
            };

            // act
            outer = JsonBinder.BindToObject(outer, json);

            // assert
            outer.One.Should().Be(10);
            outer.Config.One.Should().Be(1);
            outer.Config.Two.Should().Be(1);
            outer.Config.Three.Should().Be(1);
            outer.Config.Four.Should().Be("hello");
        }

        [Test]
        public void Should_Bind_Only_Whats_There_With_No_Specified_Section()
        {
            // arrange
            const string json = @"
                {
                    ""Config"": {
                        ""One"":1,
                        ""Two"":1,
                        ""Three"":1,
                        ""Four"":""hello""
                    },
                }";

            var outer = new OuterTestConfig();

            // act
            outer = JsonBinder.BindToObject(outer, json);

            // assert
            outer.One.Should().Be(default);
            outer.Config.One.Should().Be(1);
            outer.Config.Two.Should().Be(1);
            outer.Config.Three.Should().Be(1);
            outer.Config.Four.Should().Be("hello");
        }

        [Test]
        public void Should_Bind_Dictionary_Type()
        {
            // arrange
            const string json = @"
                {
                    ""Dictionary"":
                    {
                        ""one"": ""one"",
                        ""two"": ""two""
                    }
                }";

            var expectedDictionary = new Dictionary<string, string>()
            {
                { "one", "one" }, { "two", "two" }
            };

            var outer = new OuterTestConfig();

            // act
            outer = JsonBinder.BindToObject(outer, json);

            // assert
            outer.Dictionary.Should().BeEquivalentTo(expectedDictionary);
        }
    }
}
