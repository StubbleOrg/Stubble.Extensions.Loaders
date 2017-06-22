using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stubble.Core.Interfaces;

namespace Stubble.Extensions.Loaders
{
    public class ArrayLoader : IStubbleLoader
    {
        internal readonly IDictionary<string, string> TemplateCache;

        public ArrayLoader(IDictionary<string, string> templates)
        {
            TemplateCache = new Dictionary<string, string>(templates);
        }

        public IStubbleLoader Clone() => new ArrayLoader(TemplateCache);

        public string Load(string name)
        {
            return TemplateCache.ContainsKey(name) ? TemplateCache[name] : null;
        }

        public ValueTask<string> LoadAsync(string name) => new ValueTask<string>(Load(name));
    }
}
