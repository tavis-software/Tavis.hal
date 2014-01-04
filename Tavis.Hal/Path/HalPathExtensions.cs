using System;
using System.Collections.Generic;
using System.Linq;

namespace Tavis
{
	public static class HalPathExtensions
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IHalProperty FindProperty(this HalResource resource, string path) {
            return resource.FindNode(path) as IHalProperty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static HalResource FindResource(this HalResource resource, string path) {
            return resource.FindNode(path) as HalResource;
        }

		
        /// <summary>
        /// Selects a single node at the specified path. Path is always relative to resource.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="path"></param>
        /// <returns></returns>
		public static HalNode FindNode(this HalResource resource, string path)
		{
			object node = resource;
			foreach (var segment in new HalPath(path).Segments) {
				if (node is HalResource) {
					resource = (HalResource)node;
				}
				else {
					return null;		// Last pass resulted in non-resource node (but path not done)
				}
				
				if (!resource.ContainsNode(segment.Key)) {
					return null;
				}
				
				node = resource.GetNode(segment.Key);
			}

			return node as HalNode;
		}

	
		/// <summary>
        /// Selects all child nodes of the resource at the specified path. Path is always relative to resource.
		/// </summary>
		/// <param name="resource"></param>
		/// <param name="path"></param>
		/// <returns></returns>
        public static IEnumerable<HalNode> SelectNodesAt(this HalResource resource, string path)
		{
			var node = resource.FindNode(path);
			if ((node == null) || (!(node is HalResource))) {
				return Enumerable.Empty<HalNode>();
			}

			return ((HalResource)node).Contents;
		}



		/// <summary>
		/// relationPath specifies a unique path to a single link
		/// </summary>
        public static Link FindLink(this HalResource resource, string relationPath)
		{
			var halPath = new HalPath(relationPath);

			HalNode currentNode = resource;
			foreach (var segment in halPath.Segments) {
				if (currentNode is HalResource) {
					currentNode = ((HalResource)currentNode).GetNode(segment.Key);
				}
			}
			var halNode = currentNode;
			if (halNode is HalLink) return ((HalLink)halNode).Link;
			if (halNode is HalResource) {
				var halLink = new HalLink(((HalResource)halNode).Link); 
				//halLink.Name = halPath.Segments.Last().Name;
				return halLink.Link;
			}

			return null;
		}


		/// <summary>
		/// relationPath specifies a unique path to a single HAL resource, from which all links are found (searches deep).
		/// If relationPath is null, all links are returned
		/// </summary>
		public static IEnumerable<Link> FindAllLinks(this HalResource resource, string relationPath = null)
		{
			if (relationPath != null) {
				var halPath = new HalPath(relationPath);

				foreach (var segment in halPath.Segments) {
					var node = resource.GetNode(segment.Key) as HalResource;
					if (node == null) {
						throw new Exception("Path must specify a resource");
					}

					resource = node;
				}
			}

			var links = new List<Link>();

			InternalFindAllLinks(resource, links);

			return links;
		}


        public static IEnumerable<HalResource> FindResources(this HalResource resource, string relation)
        {
                var list = new List<HalResource>();
                FindNodesByRelation(list, relation, resource);
                return list;
            
        }

        private static  void FindNodesByRelation(List<HalResource> list, string relation, HalNode node)
        {

            var resource = node as HalResource;
            if (resource != null)
            {
                if (resource.Link.Relation == relation)
                {
                    list.Add(resource);
                }

                foreach (var content in resource.Contents)
                {
                    FindNodesByRelation(list, relation, content);
                }
            }

        }

		private static void InternalFindAllLinks(HalResource resource, IList<Link> outList)
		{
			outList.Add(resource.Link);

			foreach (var node in resource.Contents.OfType<HalLink>()) {
				outList.Add(node.Link);
			}

			foreach (var node in resource.Contents.OfType<HalResource>()) {
				InternalFindAllLinks(node, outList);
			}
		}
	}
}
