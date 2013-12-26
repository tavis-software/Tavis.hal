using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Hal;

namespace Tavis
{
    public class XmlHalReader : IHalReader
    {
        

        public HalDocument Load(Stream xmlStream)
        {
            var root = XElement.Load(xmlStream);
            return ReadAsHalDocument(root);
        }

        public HalDocument Load(String xmlhalstring)
        {
            var root = XElement.Parse(xmlhalstring);
            return ReadAsHalDocument(root);
        }


        public static HalDocument ReadAsHalDocument(XElement element)
        {
            var doc = new HalDocument(ReadAsHalResource(element,null));

            var namespaces = element.Attributes().Where(a => a.IsNamespaceDeclaration);
            foreach (var attr in namespaces)
            {
                doc.Namespaces.Add(new HalNamespace(attr.Name.LocalName, new Uri(attr.Value)));
            }

            return doc;
        }



        internal static HalResource ReadAsHalResource(XElement element, IHalResource parent)
        {

            var resource = new HalResource(
                ReadAttribute<string>(element,"rel"),
                ReadAttribute<string>(element,"href")
            )
            {
                Name = ReadAttribute<string>(element,"name"),
                Type = ReadAttribute<string>(element,"type"),
                Parent = parent
            };

            resource.Contents.Clear();
            if (resource.Type == null)
            {
                foreach (var node in element.Elements().Select(el => HalNodeFactory(el, resource)))
                {
                    resource.Contents[node.Key] = node;
                }
            }
            else
            {
                var node = new HalTypedResourceContents
                {
                    ContentNode = element
                };
                resource.Contents.Add(node.Key, node);
            }

            return resource;
        }


        internal static HalLink ReadAsHalLink(XElement element)
        {
            var link = new HalLink
            {
                Href = ReadAttribute<string>(element,"href"),
                Rel = ReadAttribute<string>(element,"rel")
            };
            return link;
        }



        private static HalNode HalNodeFactory(XElement element, IHalResource parent)
        {

            switch (element.Name.LocalName)
            {
                case "link":
                    return ReadAsHalLink(element);
                case "resource":
                    return ReadAsHalResource(element,parent);
                default:
                    return new HalProperty
                    {
                        Value = element
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