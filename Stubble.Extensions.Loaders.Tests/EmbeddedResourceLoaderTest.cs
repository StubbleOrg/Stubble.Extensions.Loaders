using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
