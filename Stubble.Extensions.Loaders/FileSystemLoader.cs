using System.Collections.Concurrent;
using System.IO;
using Stubble.Core.Interfaces;

namespace Stubble.Extensions.Loaders
{
    public class FileSystemLoader : IStubbleLoader
    {
        internal const string DefaultFileType = "mustache";
        private readonly string _path;
        private readonly string _extension;
        internal ConcurrentDictionary<string, string> TemplateCache = new ConcurrentDictionary<string, string>();

        public FileSystemLoader(string path) : this(path, DefaultFileType)
        {
        }

        public FileSystemLoader(string path, string extension)
        {
            _path = path.TrimEnd('/');
            _extension = extension;
        }

        public string Load(string name)
        {
            if (TemplateCache.ContainsKey(name)) return TemplateCache[name];

            var fileName = _path + "/name." + _extension;
            if (!File.Exists(fileName)) return null;

            var contents = File.ReadAllText(fileName);
            TemplateCache.AddOrUpdate(name, contents, (s, s1) => contents);
            return contents;
        }
    }
}
