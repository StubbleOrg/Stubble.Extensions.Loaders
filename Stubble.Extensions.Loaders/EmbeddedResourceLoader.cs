using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Interfaces;

namespace Stubble.Extensions.Loaders
{
    public class EmbeddedResourceLoader : IStubbleLoader
    {
        internal const string DefaultFileType = "mustache";
        internal readonly string Extension;
        private readonly Assembly _assembly;

        public EmbeddedResourceLoader()
        {
            _assembly = Assembly.GetCallingAssembly();
            Extension = DefaultFileType;
        }

        public EmbeddedResourceLoader(string extension)
        {
            _assembly = Assembly.GetCallingAssembly();
            Extension = extension;
        }

        public string Load(string name)
        {
            var resourceName = _assembly.GetManifestResourceNames().FirstOrDefault(rn => rn.Contains("." + name + "." + Extension));
            if (resourceName == null) return null;

            var stream = _assembly.GetManifestResourceStream(resourceName);
            if (stream == null) return null;
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
