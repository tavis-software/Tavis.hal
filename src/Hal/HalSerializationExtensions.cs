using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;


namespace Hal
{
	public static class HalSerializationExtensions
	{
		public static void WriteAsXml(this HalDocument halDocument, XmlWriter writer)
		{
			writer.WriteStartDocument();
			halDocument.Root.WriteAsXml(writer, halDocument.Namespaces);
			writer.WriteEndDocument();
		}




		public static Stream ToStream(this IHalResource resource, Stream stream = null)
		{
			if (resource is HalDocument) {
				return ((HalDocument)resource).ToStream();
			}
			
			throw new ArgumentException("Can only serialize HalDocuments (not resources)");
		}
		public static Stream ToStream(this HalDocument doc, Stream stream = null)
		{
			var wasNull = stream == null;

			stream = stream ?? new MemoryStream();

			using (var writer = XmlWriter.Create(stream)) {
				doc.WriteAsXml(writer);
			}
			
			if (wasNull) {
				stream.Position = 0;
			}

			return stream;
		}


		internal static void WriteAsXml(this HalResource halResource, XmlWriter writer, IEnumerable<HalNamespace> namespaces = null)
		{
			writer.WriteStartElement("resource");
			writer.WriteAttributeString("rel", halResource.Rel);
			if (halResource.Href != null) writer.WriteAttributeString("href", halResource.Href);
			if (!string.IsNullOrEmpty(halResource.Name)) writer.WriteAttributeString("name", halResource.Name);
			if (!string.IsNullOrEmpty(halResource.Type)) writer.WriteAttributeString("type", halResource.Type);
 
			if ((namespaces != null) && (namespaces.Any())) {
				foreach (var @namespace in namespaces) {
					writer.WriteAttributeString("xmlns", @namespace.Prefix, null, @namespace.Namespace.AsString());
				}
			}

			foreach (var halNode in halResource.Contents.Values) {
				if (halNode is HalLink) {
					var halLink = (HalLink)halNode;
					halLink.WriteAsXml(writer);
				}
				else if (halNode is HalProperty) {
					var halProperty = (HalProperty)halNode;
					halProperty.WriteAsXml(writer);
				}
				else if (halNode is HalTypedResourceContents) {
					var halTypedResourceContents = (HalTypedResourceContents)halNode;
					halTypedResourceContents.WriteAsXml(writer);
				}
				else if (halNode is HalResource) {
					var halChildResource = (HalResource)halNode;
					halChildResource.WriteAsXml(writer);
				}
			}

			writer.WriteEndElement();
		}


		internal static void WriteAsXml(this HalTypedResourceContents contents, XmlWriter writer)
		{
			foreach (var node in contents.ContentNode.DescendantNodes()) {
				node.WriteTo(writer);
			}
		}


		internal static void WriteAsXml(this HalLink halLink, XmlWriter writer)
		{
			writer.WriteStartElement("link");
			writer.WriteAttributeString("rel", halLink.Rel);
			writer.WriteAttributeString("href", halLink.Href);
			writer.WriteEndElement();

		}

		internal static void WriteAsXml(this HalProperty halProperty, XmlWriter writer)
		{
			halProperty.Value.WriteTo(writer);

		}
		


	}
}

