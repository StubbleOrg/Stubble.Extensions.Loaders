using System.Collections.Concurrent;
using System.IO;
using Stubble.Core.Interfaces;

namespace Stubble.Extensions.Loaders
{
    public class FileSystemLoader : IStubbleLoader
    {
        internal const string DefaultFileType = "mustache";
        internal readonly string Path;
        internal readonly string Extension;
        internal ConcurrentDictionary<string, string> TemplateCache = new ConcurrentDictionary<string, string>();

        public FileSystemLoader(string path) : this(path, DefaultFileType)
        {
        }

        public FileSystemLoader(string path, string extension)
        {
            Path = path.TrimEnd('/');
            Extension = extension;
        }

        public string Load(string name)
        {
            if (TemplateCache.ContainsKey(name)) return TemplateCache[name];

            var fileName = Path + "/name." + Extension;
            if (!File.Exists(fileName)) return null;

            var contents = File.ReadAllText(fileName);
            TemplateCache.AddOrUpdate(name, contents, (s, s1) => contents);
            return contents;
        }
    }
}
