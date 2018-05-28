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
        private const string DefaultFileType = "mustache";
        private readonly string Extension;
        private readonly string[] ResourceNames;
        private readonly Assembly _assembly;

        public EmbeddedResourceLoader(Assembly assembly, string extension = DefaultFileType)
        {
            _assembly = assembly;
            Extension = extension;
            ResourceNames = _assembly.GetManifestResourceNames();
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

        public async ValueTask<string> LoadAsync(string name)
        {
            var resourceName = ResourceNames.FirstOrDefault(rn => rn.Contains("." + name + "." + Extension));
            if (resourceName == null) return null;

            var stream = _assembly.GetManifestResourceStream(resourceName);
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                return await streamReader.ReadToEndAsync();
            }
        }

        public IStubbleLoader Clone() => new EmbeddedResourceLoader(_assembly, Extension);
    }
}
