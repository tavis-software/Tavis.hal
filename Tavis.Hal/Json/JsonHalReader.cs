using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tavis.IANA;

namespace Tavis
{
    public class JsonHalReader : IHalReader
    {
  
        public HalResource Load(Stream textStream)
        {

            var jsonText = new StreamReader(textStream).ReadToEnd();
            return Load(jsonText);
        }

        public HalResource Load(string jsonText)
        {

            var jObject = JObject.Parse(jsonText);

            return ExtractHalResource(jObject);
        }

        private HalResource ExtractHalResource(JObject jObject)
        {
            var namespaces = ExtractNamespaces(jObject);
            var links = ExtractLinks(jObject);
            var embedded = ExtractEmbedded(jObject);
            var properties = ExtractProperties(jObject);

            return new HalResource(new SelfLink(), links,embedded,properties);
        }

        private List<IHalProperty> ExtractProperties(JObject jObject)
        {
            throw new System.NotImplementedException();
        }

        private List<HalResource> ExtractEmbedded(JObject jObject)
        {
            throw new System.NotImplementedException();
        }

        private List<Link> ExtractLinks(JObject jObject)
        {
            throw new System.NotImplementedException();
        }

        private List<HalNamespace> ExtractNamespaces(JObject jObject)
        {
            throw new System.NotImplementedException();
        }
        

    }
}