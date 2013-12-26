using System;

namespace Tavis
{
    public class HalNamespace
    {
        public HalNamespace(string prefix, Uri @namespace)
        {
            Prefix = prefix;
            Namespace = @namespace;
        }
        public string Prefix { get; set; }
        public Uri Namespace { get; set; }
    }
}