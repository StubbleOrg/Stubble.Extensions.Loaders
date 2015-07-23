using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public string Load(string name)
        {
            return TemplateCache.ContainsKey(name) ? TemplateCache[name] : null;
        }
    }
}
