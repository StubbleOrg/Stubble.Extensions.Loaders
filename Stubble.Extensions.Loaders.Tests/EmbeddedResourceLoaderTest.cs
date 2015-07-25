using Xunit;

namespace Stubble.Extensions.Loaders.Tests
{
    public class EmbeddedResourceLoaderTest
    {
        [Fact]
        public void It_Should_Be_Able_To_Find_EmbeddedResources()
        {
            var loader = new EmbeddedResourceLoader();
            var result = loader.Load("EmbeddedFoo");
            Assert.Equal("I'm the Embedded {{foo}} template.", result);
        }

        [Fact]
        public void It_Should_Not_Throw_When_Resource_Doesnt_Exist()
        {
            var loader = new EmbeddedResourceLoader();
            var result = loader.Load("Foo");
            Assert.Null(result);
        }

        [Fact]
        public void It_Should_Work_With_Templates_In_Folders()
        {
            var loader = new EmbeddedResourceLoader();
            var result = loader.Load("EmbeddedBar");
            Assert.Equal("I'm the Embedded {{bar}} template.", result);
        }

        [Fact]
        public void It_Should_Work_With_Different_Extensions()
        {
            var loader = new EmbeddedResourceLoader("must");
            var result = loader.Load("EmbeddedBar");
            Assert.Equal("I'm the Embedded {{bar}} template.", result);
        }

        [Fact]
        public void It_Should_Allow_Assembly_To_Be_Passed()
        {
            var loader = new EmbeddedResourceLoader(GetType().Assembly);
            var result = loader.Load("EmbeddedBar");
            Assert.Equal("I'm the Embedded {{bar}} template.", result);
            var loader2 = new EmbeddedResourceLoader(GetType().Assembly, "must");
            var result2 = loader.Load("EmbeddedBar");
            Assert.Equal("I'm the Embedded {{bar}} template.", result2);
        }
    }
}
