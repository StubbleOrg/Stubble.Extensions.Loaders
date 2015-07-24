using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Stubble.Extensions.Loaders.Tests
{
    public class FileSystemLoaderTest
    {
        [Fact]
        public void It_Should_Take_Path_As_Argument()
        {
            var loader  = new FileSystemLoader("./templates");
            Assert.Equal("./templates", loader.Path);
        }

        [Fact]
        public void It_Should_Strip_Possible_Ending_Slash_On_Path()
        {
            var loader = new FileSystemLoader("./templates/");
            Assert.Equal("./templates", loader.Path);
        }
    }
}
