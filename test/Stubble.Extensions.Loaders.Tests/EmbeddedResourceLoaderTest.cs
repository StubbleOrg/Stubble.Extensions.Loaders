using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Stubble.Extensions.Loaders.Tests
{
    public class EmbeddedResourceLoaderTest
    {
        [Fact]
        public void It_Should_Be_Able_To_Find_EmbeddedResources()
        {
            var loader = new EmbeddedResourceLoader(GetType().GetTypeInfo().Assembly);
            var result = loader.Load("EmbeddedFoo");
            Assert.Equal("I'm the Embedded {{foo}} template.", result);
        }

        [Fact]
        public void It_Should_Not_Throw_When_Resource_Doesnt_Exist()
        {
            var loader = new EmbeddedResourceLoader(GetType().GetTypeInfo().Assembly);
            var result = loader.Load("Foo");
            Assert.Null(result);
        }

        [Fact]
        public async Task It_Should_Not_Throw_When_Resource_Doesnt_Exist_Async()
        {
            var loader = new EmbeddedResourceLoader(GetType().GetTypeInfo().Assembly);
            var result = await loader.LoadAsync("Foo");
            Assert.Null(result);
        }

        [Fact]
        public void It_Should_Work_With_Templates_In_Folders()
        {
            var loader = new EmbeddedResourceLoader(GetType().GetTypeInfo().Assembly);
            var result = loader.Load("EmbeddedBar");
            Assert.Equal("I'm the Embedded {{bar}} template.", result);
        }

        [Fact]
        public async Task It_Should_Work_With_Templates_In_Folders_Async()
        {
            var loader = new EmbeddedResourceLoader(GetType().GetTypeInfo().Assembly);
            var result = await loader.LoadAsync("EmbeddedBar");
            Assert.Equal("I'm the Embedded {{bar}} template.", result);
        }

        [Fact]
        public void It_Should_Work_With_Different_Extensions()
        {
            var loader = new EmbeddedResourceLoader(GetType().GetTypeInfo().Assembly, "must");
            var result = loader.Load("EmbeddedBar");
            Assert.Equal("I'm the Embedded {{bar}} template.", result);
        }

        [Fact]
        public void It_Should_Allow_Assembly_To_Be_Passed()
        {
            var loader = new EmbeddedResourceLoader(GetType().GetTypeInfo().Assembly);
            var result = loader.Load("EmbeddedBar");
            Assert.Equal("I'm the Embedded {{bar}} template.", result);
            var loader2 = new EmbeddedResourceLoader(GetType().GetTypeInfo().Assembly, "must");
            var result2 = loader.Load("EmbeddedBar");
            Assert.Equal("I'm the Embedded {{bar}} template.", result2);
        }

        [Fact]
        public void It_Should_Allow_Cloning()
        {
            var loader = new EmbeddedResourceLoader(GetType().GetTypeInfo().Assembly);

            var cloned = loader.Clone();

            Assert.NotEqual(loader, cloned);
            Assert.Equal("I'm the Embedded {{bar}} template.", loader.Load("EmbeddedBar"));
            Assert.Equal("I'm the Embedded {{bar}} template.", cloned.Load("EmbeddedBar"));
        }
    }
}
