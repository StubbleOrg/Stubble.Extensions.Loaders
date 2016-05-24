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
        internal readonly string[] ResourceNames;
        private readonly Assembly _assembly;

        public EmbeddedResourceLoader(Assembly assembly, string extension)
        {
            _assembly = assembly;
            Extension = extension;
            ResourceNames = _assembly.GetManifestResourceNames();
        }

        public EmbeddedResourceLoader(Assembly assembly) : this(assembly, DefaultFileType)
        {
        }

        public string Load(string name)
        {
            var resourceName = ResourceNames.FirstOrDefault(rn => rn.Contains("." + name + "." + Extension));
            if (resourceName == null) return null;

            var stream = _assembly.GetManifestResourceStream(resourceName);
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
