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

        [Fact]
        public void It_Should_Load_Template_From_FileSystem_With_Default_Extension()
        {
            var loader = new FileSystemLoader("./templates/");
            Assert.Equal("I'm the {{foo}} template.", loader.Load("Foo"));
        }

        [Fact]
        public void It_Should_Load_Template_From_FileSystem_With_Given_Extension()
        {
            var loader = new FileSystemLoader("./templates/", "must");
            Assert.Equal("I'm the {{bar}} template.", loader.Load("Bar"));
        }

        [Fact]
        public void It_Should_Load_Template_From_Cache_Second_Time()
        {
            var loader = new FileSystemLoader("./templates/");
            Assert.Equal("I'm the {{foo}} template.", loader.Load("Foo"));
            Assert.Equal(1, loader.TemplateCache.Count);
            Assert.Equal("I'm the {{foo}} template.", loader.Load("Foo"));
        }

        [Fact]
        public void It_Should_Skip_If_File_Doesnt_Exist()
        {
            var loader = new FileSystemLoader("./templates/");
            Assert.Null(loader.Load("Foobar"));
        }

        [Fact]
        public void It_Should_Override_On_Cache_Collision()
        {
            var loader = new FileSystemLoader("./templates/");
            var t1 = Task.Run(async () =>
            {
                await Task.Delay(500);
                loader.Load("Foo");
            });
            var t2 = Task.Run(async () =>
            {
                await Task.Delay(500);
                loader.Load("Foo");
            });
            Task.WaitAll(t1, t2);
            Assert.Equal(1, loader.TemplateCache.Count);
        }
    }
}
