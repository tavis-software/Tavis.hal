using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Tavis;

namespace Hal {

    public class HalNamespace
    {
        public HalNamespace(string prefix, Uri @namespace)
        {
            Prefix = prefix;
            Namespace = @namespace;
        }
        public string Prefix { get; set; }
        public Uri Namespace { get; set; }
    }

	public class HalDocument : HalNode, IHalResource
	{
		public HalResource Root { get; internal set; }

        public readonly IList<HalNamespace> Namespaces;

		public string Rel {
			get { return Root.Rel; }
			set { Root.Rel = value; }
		}
		
		public string Href {
			get { return Root.Href; }
			set { Root.Href = value; }
		}
		
		public string Name {
			get { return Root.Name; }
			set { Root.Name = value; }
		}
		
		public string Type {
			get { return Root.Type; }
			set { Root.Type = value; }
		}

		public OrderedDictionary<string, HalNode> Contents { get { return Root.Contents; } }

		public override string Key {
			get { return Root.Key; }
		}

		public IHalResource Parent { get { return null; } }


		internal HalDocument(HalResource root)
			: this((string)null)
		{
			Root = root;

		}
		
		public HalDocument(Uri href, string rel = "self")
			: this(href.AsString(), rel)
		{
		}

    	public HalDocument(string href, string rel = "self")
    	{
            Namespaces = new List<HalNamespace>();
			Root = new HalResource(rel, href);
    	}


        public static HalDocument Load(Stream stream) {
            return new XmlHalReader().Load(stream);// XElement.Load(stream).ReadAsHalDocument();
        }


        public static HalDocument Parse(string hal) {
            return new XmlHalReader().Load(hal);
        }

	    
    }

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
                if (resource.ResourceLink.Rel == relation) {
                    list.Add(resource);
                }

                foreach (var content in resource.Contents.Values)
                {
                    FindNodesByRelation(list, relation, content);
                }
            }

        }

    }

    public class PropertyFinder
    {
        private readonly HalResource _root;

        public PropertyFinder(HalResource root)
        {
            _root = root;
        }

        public string this[string propertyNameAndPath]
        {
            get {
                var parts = propertyNameAndPath.Split('/');
                var propertyName = parts[0];
                var prop = _root.FindProperty(propertyName);
                var xValue = prop.Value;
                
                if (parts.Length > 1 ) {
                    var xPath = parts[1];

                    var result = (object)"";  //TODO xValue.XPathEvaluate(xPath);
                    if (result is string) {
                        return (string) result;
                    } else 
                    if (result is double) {
                        return ((double) result).ToString();
                    } else {
                        var att = (IEnumerable) result;
                        return att.Cast<XAttribute>().FirstOrDefault().Value;
                    }

                    
                } else {
                    return xValue.Value;    
                    
                }
                
            }
        }
    }

}
