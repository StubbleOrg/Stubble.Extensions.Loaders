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
        private const string DefaultFileType = "mustache";
        private const char DefaultDelimiter = ':';
        private static readonly char DirectorySeparatorChar = Path.DirectorySeparatorChar;
        internal readonly string BasePath;
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
            BasePath = path.TrimEnd('/', '\\');
            Extension = extension;
            Delimiter = delimiter;
        }

        public IStubbleLoader Clone() => new FileSystemLoader(BasePath, Delimiter, Extension);

        public string Load(string name)
        {
            if (TemplateCache.ContainsKey(name))
            {
                return TemplateCache[name];
            }

            var fileName = BuildFilePath(name);

            if (!File.Exists(fileName)) return null;

            var contents = File.ReadAllText(fileName);

            AddToTemplateCache(name, contents);
            return contents;
        }

        public async ValueTask<string> LoadAsync(string name)
        {
            if (TemplateCache.ContainsKey(name))
            {
                return TemplateCache[name];
            }

            var fileName = BuildFilePath(name);
            if (!File.Exists(fileName)) return null;

            var contents = await LoadFileAsync(fileName);

            AddToTemplateCache(name, contents);
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

            if (Delimiter != DirectorySeparatorChar)
            {
                var split = name.Split(Delimiter).Select(s => s.Trim()).ToArray();

                filePath = string.Join(DirectorySeparatorChar.ToString(), split);
            }

            return string.IsNullOrEmpty(Extension)
                ? Path.Combine(BasePath, filePath)
                : Path.Combine(BasePath, filePath + "." + Extension);
        }

        internal void AddToTemplateCache(string name, string contents)
        {
            TemplateCache.AddOrUpdate(name, contents, (s, s1) => contents);
        }
    }
}
