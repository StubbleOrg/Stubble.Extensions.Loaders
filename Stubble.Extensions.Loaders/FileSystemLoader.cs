using System;
using System.Collections.Generic;
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
        private readonly string _fileType;
        internal IDictionary<string, string> TemplateCache = new Dictionary<string, string>(10); 

        public FileSystemLoader(string path) : this(path, DefaultFileType)
        {
        }

        public FileSystemLoader(string path, string fileType)
        {
            _path = path;
            _fileType = fileType;
        }

        public string Load(string name)
        {
            throw new NotImplementedException();
        }
    }
}
