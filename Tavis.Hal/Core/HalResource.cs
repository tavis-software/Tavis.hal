using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tavis.IANA;

namespace Tavis {

    public class HalResource : HalNode {

	    public string Name { get; set; }
        public Link Link { get; set; }
        
        internal readonly IList<HalNamespace> Namespaces;

        public HalResource() : this(null)
        {
        }

        public HalResource(Link resourceLink, params object[] contents)
        {

            Namespaces = new List<HalNamespace>();
            Link = resourceLink;
            Flatten(contents);
        }

        public void AddNamespace(string name, Uri uri)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (uri == null) throw new ArgumentNullException("uri");
            if (Parent != null) throw new Exception("Namespaces can only be added to the root resource");

            Namespaces.Add(new HalNamespace(name, uri));
            
        }
        

        internal void Flatten(IEnumerable content)
        {
            foreach (var o in content)
            {
                if (o is IEnumerable)
                {
                    Flatten((IEnumerable)o);
                }
                else
                {
                    HalNode node;
                    var link = o as Link;
                    if (link != null)
                    {
                        if (link.Target == null) throw new ArgumentException("Link with relation " + link.Relation + " has no Target");
                        node = new HalLink(link);
                    }
                    else
                    {
                        node = (HalNode)o;
                    }
                    AddNode(node);
                }
            }
        }
        public override string Key {
            get {
                return Link.Relation + (string.IsNullOrEmpty(Name) ? string.Empty : "[" + Name + "]");
            }
        }

        public IEnumerable<HalNode > Contents
        {
            get { return _Contents; }
        }

        public void AddNode(Link link)
        {
            AddNode(new HalLink(link));
        }

        public  void AddNode(HalNode node)
        {
            if ((node is HalResource) && (((HalResource)node).Name == null))
            {
                var newChild = (HalResource)node;

                var ofSameRel = _Contents.OfType<HalResource>().Where(r => r.Link.Relation == newChild.Link.Relation);
                if (ofSameRel.Any())
                {
                    var maxName = ofSameRel.Max(r =>
                    {
                        if (r.Name == null)
                        {
                            return null;
                        }

                        int name;
                        if (int.TryParse(r.Name, NumberStyles.Integer, CultureInfo.InvariantCulture, out name))
                        {
                            return name;
                        }

                        return -1;
                    });

                    if (!maxName.HasValue)
                    {
                        // All resources with same rel have no name; that means there's only one resource with the same rel (names are unique)
                        var oldChild = ofSameRel.First();

                        oldChild.Name = "1";
                        newChild.Name = "2";
                    }
                    else
                    {
                        if (maxName == -1)
                        {
                            throw new Exception("Cannot insert node with duplicate rel; unique name cannot be determined since other nodes have non-numeric names");
                        }

                        newChild.Name = (maxName + 1).Value.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }

            if (ContainsNode(node.Key))
            {
                throw new Exception("Node rel+name is not unique within parent resource");
            }

            _Contents.Add(node);
           
        }

        public HalNode GetNode(string key)
        {
            return _Contents.FirstOrDefault(n => n.Key == key);
        }
        public bool ContainsNode(string key)
        {
            return _Contents.Any(n => n.Key == key);
        }
    }
}
