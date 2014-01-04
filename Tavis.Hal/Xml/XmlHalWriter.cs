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

        public void CopyToStream(HalResource document, Stream stream)
        {
            using (var writer = XmlWriter.Create(stream,new XmlWriterSettings(){Indent = true}))
            {
                WriteAsXml(document, writer);
            }
        }

        public Stream ToStream(HalResource document)
        {
            var stream = new MemoryStream();

            using (var writer = XmlWriter.Create(stream))
            {
                WriteAsXml(document, writer);
            }
            
            stream.Position = 0;

            return stream;
        }

        

        

        internal static void WriteAsXml(HalResource halResource, XmlWriter writer)
        {
            writer.WriteStartElement("resource");
            writer.WriteAttributeString("rel", halResource.Link.Relation);

            if (halResource.Link.Target != null) writer.WriteAttributeString("href", halResource.Link.Target.OriginalString);
            if (halResource.Link.Type != null) writer.WriteAttributeString("type", halResource.Link.Type.ToString());

            if (!string.IsNullOrEmpty(halResource.Name)) writer.WriteAttributeString("name", halResource.Name);

            var namespaces = halResource.Namespaces;
            if ((namespaces != null) && (namespaces.Any()))
            {
                foreach (var @namespace in namespaces)
                {
                    writer.WriteAttributeString("xmlns", @namespace.Prefix, null, @namespace.Namespace.OriginalString);
                }
            }

            foreach (var halNode in halResource.Contents)
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
            var property = new XElement(halProperty.Name);
            property.Value = halProperty.GetValue() as string;
            property.WriteTo(writer);
            

        }


  
    }
}
