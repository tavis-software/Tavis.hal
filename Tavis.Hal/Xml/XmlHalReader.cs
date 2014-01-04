using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Xml.Linq;

namespace Tavis
{
    public class XmlHalReader : IHalReader
    {
        private readonly LinkFactory _linkFactory;

        public XmlHalReader(LinkFactory linkFactory = null)
        {
            _linkFactory = linkFactory ?? new LinkFactory();
        }

        public HalResource Load(Stream xmlStream)
        {
            var root = XElement.Load(xmlStream);
            return ReadAsHalResource(root,null);
        }

        public HalResource Load(String xmlhalstring)
        {
            var root = XElement.Parse(xmlhalstring);
            return ReadAsHalResource(root,null);
        }


      

        internal HalResource ReadAsHalResource(XElement element, HalResource parent)
        {
            
            var resource = new HalResource(ReadAsLink(element))
            {
                Name = ReadAttribute<string>(element,"name"),
                Parent = parent
            };

            var namespaces = element.Attributes().Where(a => a.IsNamespaceDeclaration);
            foreach (var attr in namespaces)
            {
                resource.Namespaces.Add(new HalNamespace(attr.Name.LocalName, new Uri(attr.Value)));
            }

            
            if (resource.Link.Type == null)
            {
                foreach (var node in element.Elements().Select(el => HalNodeFactory(el, resource)))
                {
                    resource.AddNode(node);
                }
            }
            else
            {
                var node = new HalTypedResourceContents
                {
                    ContentNode = element
                };
                resource.AddNode(node);
            }

            return resource;
        }

        internal Link ReadAsLink(XElement element)
        {
            var relation = ReadAttribute<string>(element, "rel");
            var link = _linkFactory.CreateLink(relation);
            link.Target = new Uri(ReadAttribute<string>(element, "href"), UriKind.RelativeOrAbsolute);
            
            var type = ReadAttribute<string>(element, "type");
            if (!String.IsNullOrEmpty(type))
            {
                link.Type = new MediaTypeHeaderValue(type);     
            }
            
            return link;
        }

        internal HalLink ReadAsHalLink(XElement element)
        {
            return new HalLink(ReadAsLink(element));
        }


        private HalNode HalNodeFactory(XElement element, HalResource parent)
        {

            switch (element.Name.LocalName)
            {
                case "link":
                    return ReadAsHalLink(element);
                case "resource":
                    return ReadAsHalResource(element,parent);
                default:
                    return new HalXProperty
                    {
                        Content = element
                    };
            }
        }


        private static T ReadAttribute<T>(XElement element, string name)
        {
            if (element.Attribute(name) == null) return default(T);
            return (T)Convert.ChangeType(element.Attribute(name).Value, typeof(T), Thread.CurrentThread.CurrentCulture);
        }

    }
}