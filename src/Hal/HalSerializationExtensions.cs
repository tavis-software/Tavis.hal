using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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


		internal static void WriteAsXml(this HalResource halResource, XmlWriter writer, IEnumerable<Tuple<string, Uri>> namespaces = null)
		{
			writer.WriteStartElement("resource");
			writer.WriteAttributeString("rel", halResource.Rel);
			if (halResource.Href != null) writer.WriteAttributeString("href", halResource.Href);
			if (!string.IsNullOrEmpty(halResource.Name)) writer.WriteAttributeString("name", halResource.Name);
			if (!string.IsNullOrEmpty(halResource.Type)) writer.WriteAttributeString("type", halResource.Type);
 
			if ((namespaces != null) && (namespaces.Any())) {
				foreach (var @namespace in namespaces) {
					writer.WriteAttributeString("xmlns", @namespace.Item1, null, @namespace.Item2.AsString());
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


		internal static void WriteAsXml(this HalTypedResourceContents contents, XmlWriter writer)		{			foreach (var node in contents.ContentNode.DescendantNodes()) {				node.WriteTo(writer);			}		}


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
		


		public static HalDocument ReadAsHalDocument(this XElement element)
		{
			var doc = new HalDocument(element.ReadAsHalResource(null));

			var namespaces = element.Attributes().Where(a => a.IsNamespaceDeclaration);
			foreach (var attr in namespaces) {
				doc.Namespaces.Add(new Tuple<string, Uri>(attr.Name.LocalName, new Uri(attr.Value)));
			}

			return doc;
		}
		

		
		internal static HalResource ReadAsHalResource(this XElement element, IHalResource parent)
		{

			var resource = new HalResource(
				element.ReadAttribute<string>("rel"),
				element.ReadAttribute<string>("href")
			) {
				Name = element.ReadAttribute<string>("name"),
				Type = element.ReadAttribute<string>("type"),
				Parent = parent
			};

			resource.Contents.Clear();
			if (resource.Type == null) {
				foreach (var node in element.Elements().Select(el => HalNodeFactory(el, resource))) {
					resource.Contents[node.Key] = node;
				}
			}
			else {
				var node = new HalTypedResourceContents {
					ContentNode = element
				};
				resource.Contents.Add(node.Key, node);
			}
			
			return resource;
		}


		internal static HalLink ReadAsHalLink(this XElement element)
		{
			var link = new HalLink {
				Href = element.ReadAttribute<string>("href"),
				Rel = element.ReadAttribute<string>("rel")
			};
			return link;
		}



		private static HalNode HalNodeFactory(XElement element, IHalResource parent)
		{

			switch (element.Name.LocalName) {
				case "link":
					return element.ReadAsHalLink();
				case "resource":
					return element.ReadAsHalResource(parent);
				default:
					return new HalProperty {
						Value = element
					};
			}
		}


		private static T ReadAttribute<T>(this XElement element, string name)
		{
			if (element.Attribute(name) == null) return default(T);
			return (T)Convert.ChangeType(element.Attribute(name).Value, typeof(T));
		}

	}
}

