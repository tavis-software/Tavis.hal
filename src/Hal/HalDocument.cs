using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Hal {
	public class HalDocument : HalNode, IHalResource
	{
		internal HalResource Root { get; set; }

		public readonly IList<Tuple<string, Uri>> Namespaces;

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
    		Namespaces = new List<Tuple<string, Uri>>();
			Root = new HalResource(rel, href);
    	}


        public static HalDocument Load(Stream stream) {
            return XElement.Load(stream).ReadAsHalDocument();
        }


        public static HalDocument Parse(string hal) {
            return XElement.Parse(hal).ReadAsHalDocument();
        }
    }
}
