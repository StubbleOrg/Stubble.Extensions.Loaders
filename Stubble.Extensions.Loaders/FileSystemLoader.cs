using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Interfaces;

namespace Stubble.Extensions.Loaders
{
    public class FileSystemLoader : IStubbleLoader
    {
        internal const string DefaultFileType = "mustache";
        private readonly string _path;
        private readonly string _extension;
        internal IDictionary<string, string> TemplateCache = new Dictionary<string, string>(10); 

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
            TemplateCache.Add(name, contents);
            return contents;
        }
    }
}
