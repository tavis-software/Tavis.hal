using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Tavis {

    public class XmlHalWriter : IHalWriter
    {

        public void CopyToStream(HalDocument document, Stream stream)
        {
            using (var writer = XmlWriter.Create(stream))
            {
                WriteAsXml(document, writer);
            }
        }

        public Stream ToStream(HalDocument document)
        {
            var stream = new MemoryStream();

            using (var writer = XmlWriter.Create(stream))
            {
                WriteAsXml(document, writer);
            }
            
            stream.Position = 0;

            return stream;
        }

        

        public static void WriteAsXml(HalDocument halDocument, XmlWriter writer)
        {
            writer.WriteStartDocument();
            WriteAsXml(halDocument.Root, writer, halDocument.Namespaces);
            writer.WriteEndDocument();
        }

        internal static void WriteAsXml(HalResource halResource, XmlWriter writer, IEnumerable<HalNamespace> namespaces = null)
        {
            writer.WriteStartElement("resource");
            writer.WriteAttributeString("rel", halResource.Rel);
            if (halResource.Href != null) writer.WriteAttributeString("href", halResource.Href);
            if (!string.IsNullOrEmpty(halResource.Name)) writer.WriteAttributeString("name", halResource.Name);
            if (!string.IsNullOrEmpty(halResource.Type)) writer.WriteAttributeString("type", halResource.Type);

            if ((namespaces != null) && (namespaces.Any()))
            {
                foreach (var @namespace in namespaces)
                {
                    writer.WriteAttributeString("xmlns", @namespace.Prefix, null, @namespace.Namespace.AsString());
                }
            }

            foreach (var halNode in halResource.Contents.Values)
            {
                if (halNode is HalLink)
                {
                    var halLink = (HalLink)halNode;
                    WriteAsXml(halLink, writer);
                }
                else if (halNode is IHalProperty)
                {
                    var halProperty = (IHalProperty)halNode;
                    WriteAsXml(halProperty,writer);
                }
                else if (halNode is HalTypedResourceContents)
                {
                    var halTypedResourceContents = (HalTypedResourceContents)halNode;
                    WriteAsXml(halTypedResourceContents,writer);
                }
                else if (halNode is HalResource)
                {
                    var halChildResource = (HalResource)halNode;
                    WriteAsXml(halChildResource, writer);
                }
            }

            writer.WriteEndElement();
        }


        internal static void WriteAsXml(HalTypedResourceContents contents, XmlWriter writer)
        {
            foreach (var node in contents.ContentNode.DescendantNodes())
            {
                node.WriteTo(writer);
            }
        }


        internal static void WriteAsXml(HalLink halLink, XmlWriter writer)
        {
            writer.WriteStartElement("link");
            writer.WriteAttributeString("rel", halLink.Rel);
            writer.WriteAttributeString("href", halLink.Href);
            writer.WriteEndElement();

        }

        internal static void WriteAsXml(IHalProperty halProperty, XmlWriter writer)
        {
            var content = halProperty.GetContent() as XElement;
            if (content != null)
            {
              content.WriteTo(writer);    
            }
            

        }


  
    }
}
