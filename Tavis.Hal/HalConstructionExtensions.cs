using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

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

        
        public static IHalResource AddLink(this IHalResource resource, string rel, string href)
        {
            return resource.AddLink(new Link() {Relation = rel, Target = href.AsUri()});
        }

        public static IHalResource AddLink(this IHalResource resource, Link link)
        {
            var hallink = new HalLink(link);

            return resource.AddNode(hallink);
        }


        public static IHalResource AddJProperty(this IHalResource resource, string name, object value)
        {
            var jProperty = new JProperty(name, value);


            return resource.AddNode(new HalJProperty { Content = jProperty });

        }
        public static IHalResource AddXProperty(this IHalResource resource, string name, string value)
        {
            var element = new XElement(name, value);

            return resource.AddNode(new HalXProperty {Content = element});
        }


        public static IHalResource CreateResource(this IHalResource resource, string relation, string href , string name = null)
        {
            return resource.CreateResource(new Link() {Relation = relation, Target = new Uri(href, UriKind.RelativeOrAbsolute)});
        }

    public static IHalResource CreateResource(this IHalResource resource, Link resourceLink, string name = null)
		{
            var newResource = new HalResource(resourceLink)
            {
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


		public static IHalResource AddTypedResource(this IHalResource resource, Link resourceLink, object contents, string name = null)
		{
			var newResource = resource.CreateResource(resourceLink, name);
			newResource.Type = resourceLink.Type.MediaType;

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
