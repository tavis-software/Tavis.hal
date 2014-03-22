using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tavis.IANA;

namespace Tavis
{
    public class JsonHalReader : IHalReader
    {
  
       private readonly LinkFactory _linkFactory;

        public JsonHalReader(LinkFactory linkFactory = null)
        {
            _linkFactory = linkFactory ?? new LinkFactory();
        }

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

        private HalResource ExtractHalResource(JObject jObject, Link link = null)
        {
            if (link == null) link = new SelfLink();
            var namespaces = ExtractNamespaces(jObject);
            var links = ExtractLinks(jObject.Property("_links"));
            var embedded = ExtractEmbedded(jObject.Property("_embedded"));
            var properties = ExtractProperties(jObject);

            return new HalResource(link, links,embedded,properties);
        }

        private List<HalProperty> ExtractProperties(JObject jObject)
        {
            var props =
                jObject.Properties().Where(p => p.Name != "_curies" && p.Name != "_links" && p.Name != "_embedded")
                    .Select(p => new HalProperty(p.Name, (string) p.Value));
            return props.ToList();
        }

        private List<HalResource> ExtractEmbedded(JProperty jProperty)
        {
            if (jProperty == null) return new List<HalResource>();
            var resourcesObject = jProperty.Value as JObject;

            var resources = resourcesObject.Properties()
                   .SelectMany(p =>
                   {
                       var link = _linkFactory.CreateLink((string) p.Name);
                       var jResource = p.Value as JObject;

                       if (jResource != null)
                       {
                           return new List<HalResource> {ExtractHalResource(jResource, link)};
                       }
                       else
                       {
                           var aResource = p.Value as JArray;
                           return aResource.Values().Cast<JObject>().Select(r => ExtractHalResource(r, link));
                       }
                   });
            return resources.ToList();
        }

        private List<Link> ExtractLinks(JProperty jProperty)
        {
            if (jProperty == null) return new List<Link>();

            var linksObject = jProperty.Value as JObject;

            var links =linksObject.Properties()
                   .Select(p =>
                   {
                       var jlink = p.Value as JObject;
                       var link = _linkFactory.CreateLink((string) p.Name);
                       link.Target = new Uri((string) jlink["href"]);
                       return link;
                   });
            return links.ToList();
        }

        private List<HalNamespace> ExtractNamespaces(JObject jObject)
        {
            return new List<HalNamespace>();
        }
        

    }
}