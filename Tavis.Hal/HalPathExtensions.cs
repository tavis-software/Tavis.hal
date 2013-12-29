using System;
using System.Collections.Generic;
using System.Linq;

namespace Tavis
{
	public static class HalPathExtensions
	{
        public static IHalProperty FindProperty(this IHalResource resource, string path) {
            return resource.FindNode(path) as IHalProperty;
        }
        public static HalResource FindResource(this IHalResource resource, string path) {
            return resource.FindNode(path) as HalResource;
        }

		// Selects a single node at the specified path. Path is always relative to resource.
		public static HalNode FindNode(this IHalResource resource, string path)
		{
			object node = resource;
			foreach (var segment in new HalPath(path).Segments) {
				if (node is IHalResource) {
					resource = (IHalResource)node;
				}
				else {
					return null;		// Last pass resulted in non-resource node (but path not done)
				}
				
				if (!resource.Contents.ContainsKey(segment.Key)) {
					return null;
				}
				
				node = resource.Contents[segment.Key];
			}

			return node as HalNode;
		}


		// Selects all child nodes of the resource at the specified path. Path is always relative to resource.
		public static IEnumerable<HalNode> SelectNodesAt(this IHalResource resource, string path)
		{
			var node = resource.FindNode(path);
			if ((node == null) || (!(node is IHalResource))) {
				return Enumerable.Empty<HalNode>();
			}

			return ((IHalResource)node).Contents.Values;
		}



		/// <summary>relationPath specifies a unique path to a single link</summary>
		public static HalLink FindLink(this HalDocument document, string relationPath)
		{
			var halPath = new HalPath(relationPath);

			HalNode currentNode = document.Root;
			foreach (var segment in halPath.Segments) {
				if (currentNode is HalResource) {
					currentNode = ((HalResource)currentNode).Contents[segment.Key];
				}
			}
			var halNode = currentNode;
			if (halNode is HalLink) return (HalLink)halNode;
			if (halNode is HalResource) {
				var halLink = new HalLink(((HalResource)halNode).Link); 
				//halLink.Name = halPath.Segments.Last().Name;
				return halLink;
			}

			return null;
		}


		/// <summary>relationPath specifies a unique path to a single HAL resource, from which all links are found (searches deep).
		/// If relationPath is null, all links are returned</summary>
		public static IEnumerable<HalLink> FindAllLinks(this HalDocument document, string relationPath = null)
		{
			var resource = document.Root;
			if (relationPath != null) {
				var halPath = new HalPath(relationPath);

				foreach (var segment in halPath.Segments) {
					var node = resource.Contents[segment.Key] as HalResource;
					if (node == null) {
						throw new Exception("Path must specify a resource");
					}

					resource = node;
				}
			}

			var links = new List<HalLink>();

			InternalFindAllLinks(resource, links);

			return links;
		}


		private static void InternalFindAllLinks(HalResource resource, IList<HalLink> outList)
		{
			outList.Add(new HalLink(resource.Link));

			foreach (var node in resource.Contents.Values.OfType<HalLink>()) {
				outList.Add(node);
			}

			foreach (var node in resource.Contents.Values.OfType<HalResource>()) {
				InternalFindAllLinks(node, outList);
			}
		}
	}
}
