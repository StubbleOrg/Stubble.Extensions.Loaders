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

        public EmbeddedResourceLoader(Assembly assembly, string extension)
        {
            _assembly = assembly;
            Extension = extension;
        }

        public EmbeddedResourceLoader()
            : this(Assembly.GetCallingAssembly(), DefaultFileType)
        {
        }

        public EmbeddedResourceLoader(string extension)
            : this(Assembly.GetCallingAssembly(), extension)
        {
        }

        public string Load(string name)
        {
            var resourceName = _assembly.GetManifestResourceNames().FirstOrDefault(rn => rn.Contains("." + name + "." + Extension));
            if (resourceName == null) return null;

            var stream = _assembly.GetManifestResourceStream(resourceName);
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
