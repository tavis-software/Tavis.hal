using System;
using System.Collections.Generic;


namespace Tavis {
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

		

		public override string Key {
			get { return Root.Key; }
		}

        public OrderedDictionary<string, HalNode> Contents { get { return Root.Contents; } }
		public IHalResource Parent { get { return null; } }


		internal HalDocument(HalResource root) : this((string)null)
		{
			Root = root;
		}
		
		public HalDocument(Uri href, string rel = "self") : this(href.AsString(), rel)
		{
		}

    	public HalDocument(string href, string rel = "self")
    	{
            Namespaces = new List<HalNamespace>();
			Root = new HalResource(rel, href);
    	}


	}
}
