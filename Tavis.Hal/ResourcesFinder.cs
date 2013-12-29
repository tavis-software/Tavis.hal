using System.Collections.Generic;

namespace Tavis
{
    public class ResourcesFinder {
        private readonly HalNode _root;

        public ResourcesFinder(HalNode root) {
            _root = root;
        }

        public IEnumerable<HalResource> this[string relation] {
            get {
                var list = new List<HalResource>();
                FindNodesByRelation(list,relation,_root);
                return list;
            }       
        }

        private void FindNodesByRelation(List<HalResource> list, string relation, HalNode node) {
            
            var resource = node as HalResource;
            if (resource != null) {
                if (resource.Link.Relation == relation) {
                    list.Add(resource);
                }

                foreach (var content in resource.Contents.Values)
                {
                    FindNodesByRelation(list, relation, content);
                }
            }

        }

    }
}