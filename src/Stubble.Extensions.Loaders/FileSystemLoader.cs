using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Stubble.Core.Interfaces;

namespace Stubble.Extensions.Loaders
{
    public class FileSystemLoader : IStubbleLoader
    {
        internal const string DefaultFileType = "mustache";
        internal const char DefaultDelimiter = ':';
        internal readonly string Path;
        internal readonly string Extension;
        internal readonly char Delimiter;
        internal ConcurrentDictionary<string, string> TemplateCache = new ConcurrentDictionary<string, string>();

        public FileSystemLoader(string path) : this(path, DefaultDelimiter, DefaultFileType)
        {
        }

        public FileSystemLoader(string path, string extension) : this(path, DefaultDelimiter, extension)
        {
        }

        public FileSystemLoader(string path, char delimiter) : this(path, delimiter, DefaultFileType)
        {
        }

        public FileSystemLoader(string path, char delimiter, string extension)
        {
            Path = path.TrimEnd('/', '\\');
            Extension = extension;
            Delimiter = delimiter;
        }

        public string Load(string name)
        {
            if (TemplateCache.ContainsKey(name)) return TemplateCache[name];
            var split = name.Split(Delimiter).Select(s => s.Trim()).ToArray();
            var route = $"{Path}";
            while (split.Length > 1)
            {
                if (Directory.Exists($@"{Path}\{split[0]}"))
                {
                    route += $@"\{split[0]}";
                    split = split.Skip(1).ToArray();
                }
                else
                {
                    return null;
                }
            }

            var fileName = route + "/" + split.First() + "." + Extension;
            if (!File.Exists(fileName)) return null;
            var contents = File.ReadAllText(fileName);
            AddToTemplateCache(name, contents);
            return contents;
        }

        internal void AddToTemplateCache(string name, string contents)
        {
            TemplateCache.AddOrUpdate(name, contents, (s, s1) => contents);
        }
    }
}
