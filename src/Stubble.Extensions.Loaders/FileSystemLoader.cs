using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Stubble.Core.Interfaces;
using System.Text;

namespace Stubble.Extensions.Loaders
{
    public class FileSystemLoader : IStubbleLoader
    {
        internal const string DefaultFileType = "mustache";
        internal const char DefaultDelimiter = ':';
        internal static char DirectorySeparatorChar = System.IO.Path.DirectorySeparatorChar;
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

        public IStubbleLoader Clone() => new FileSystemLoader(Path, Delimiter, Extension);

        public string Load(string name)
        {
            if (TemplateCache.ContainsKey(name)) return TemplateCache[name];

            var fileName = BuildFilePath(name);

            if (!File.Exists(fileName)) return null;

            var contents = File.ReadAllText(fileName);

            AddToTemplateCache(name, contents);
            return contents;
        }

        public async ValueTask<string> LoadAsync(string name)
        {
            if (TemplateCache.ContainsKey(name)) return TemplateCache[name];

            var fileName = BuildFilePath(name);
            if (!File.Exists(fileName)) return null;

            var contents = await LoadFileAsync(fileName);

            return contents;

            async Task<string> LoadFileAsync(string filePath) {
                using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
                using (var reader = new StreamReader(file, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 4096, leaveOpen: true))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        internal string BuildFilePath(string name)
        {
            var filePath = name;

            if (!(Delimiter == DirectorySeparatorChar))
            {
                var split = name.Split(Delimiter).Select(s => s.Trim()).ToArray();

                filePath = string.Join(DirectorySeparatorChar.ToString(), split);
            }

            return System.IO.Path.Combine(Path, filePath + "." + Extension);
        }

        internal void AddToTemplateCache(string name, string contents)
        {
            TemplateCache.AddOrUpdate(name, contents, (s, s1) => contents);
        }
    }
}
