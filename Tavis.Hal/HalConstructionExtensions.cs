using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Tavis
{
	public static class HalConstructionExtensions
	{
		public static HalDocument AddNamespace(this HalDocument document, string name, string uri)
		{
			return AddNamespace(document, name, uri.AsUri());
		}
		public static HalDocument AddNamespace(this HalDocument document, string name, Uri uri)
		{
			if (name == null) throw new ArgumentNullException("name");
			if (uri == null) throw new ArgumentNullException("uri");

			document.Namespaces.Add(new HalNamespace(name, uri));
			return document;
		}


        public static IHalResource AddLink(this IHalResource resource, Link link)
        {
            return AddLink(resource, link.Relation, link.Target);
        }
		
		public static IHalResource AddLink(this IHalResource resource, string rel, string href)
		{
			return AddLink(resource, rel, href.AsUri());
		}
		public static IHalResource AddLink(this IHalResource resource, string rel, Uri href)
		{
			var link = new HalLink {
				Rel = rel,
				Href = href.AsString()
			};

			return resource.AddNode(link);
		}


		public static IHalResource AddProperty(this IHalResource resource, string name, string value)
		{
			var element = new XElement(name, value);

			return resource.AddXml(element);
		}


		public static IHalResource AddXml(this IHalResource resource, XElement element)
		{
			return resource.AddNode(new HalXProperty { Content = element });
		}


		public static IHalResource CreateResource(this IHalResource resource, string rel, string href, string name = null)
		{
			return CreateResource(resource, rel, href.AsUri(), name);
		}
		public static IHalResource CreateResource(this IHalResource resource, string rel, Uri href = null, string name = null)
		{
			var newResource = new HalResource(rel, href) {
				Name = name,
				Parent = resource
			};

			resource.AddNode(newResource);

			return newResource;

		}
		public static IHalResource End(this IHalResource resource)
		{
			return resource.Parent;
		}


		public static IHalResource AddTypedResource(this IHalResource resource, string rel, string type, string contents, string href, string name = null)
		{
			return AddTypedResource(resource, rel, type, (object)contents, href.AsUri(), name);
		}
		public static IHalResource AddTypedResource(this IHalResource resource, string rel, string type, string contents, Uri href = null, string name = null)
		{
			return AddTypedResource(resource, rel, type, (object)contents, href, name);
		}
		public static IHalResource AddTypedResource(this IHalResource resource, string rel, string type, XElement contents, string href, string name = null)
		{
			return AddTypedResource(resource, rel, type, (object)contents, href.AsUri(), name);
		}
		public static IHalResource AddTypedResource(this IHalResource resource, string rel, string type, XElement contents, Uri href = null, string name = null)
		{
			return AddTypedResource(resource, rel, type, (object)contents, href, name);
		}
		internal static IHalResource AddTypedResource(this IHalResource resource, string rel, string type, object contents, Uri href = null, string name = null)
		{
			var newResource = resource.CreateResource(rel, href, name);
			newResource.Type = type;

			var resourceContents = new HalTypedResourceContents {
				ContentNode = contents is XElement ? (XElement)contents : new XElement("TypedResourceContentsWrapper", contents)
			};
			newResource.Contents.Add(resourceContents.Key, resourceContents);

			return newResource.End();
		}


		public static IHalResource AddNode(this IHalResource resource, HalNode node)
		{
			if ((node is HalResource) && (((HalResource)node).Name == null)) {
				var newChild = (HalResource)node;

				var ofSameRel = resource.Contents.Values.OfType<HalResource>().Where(r => r.Rel == newChild.Rel);
				if (ofSameRel.Any()) {
					var maxName = ofSameRel.Max(r => {
						if (r.Name == null) {
							return null;
						}

						int name;
						if (int.TryParse(r.Name, NumberStyles.Integer, CultureInfo.InvariantCulture, out name)) {
							return name;
						}

						return -1;
					});

					if (!maxName.HasValue) {
						// All resources with same rel have no name; that means there's only one resource with the same rel (names are unique)
						var oldChild = ofSameRel.First();

						oldChild.Name = "1";
						newChild.Name = "2";
					}
					else {
						if (maxName == -1) {
							throw new Exception("Cannot insert node with duplicate rel; unique name cannot be determined since other nodes have non-numeric names");
						}

						newChild.Name = (maxName + 1).Value.ToString(CultureInfo.InvariantCulture);
					}
				}
			}
			
			if (resource.Contents.ContainsKey(node.Key)) {
				throw new Exception("Node rel+name is not unique within parent resource");
			}

			resource.Contents.Add(node.Key, node);
			return resource;
		}
	}
}
