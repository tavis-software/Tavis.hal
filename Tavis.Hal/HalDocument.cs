using System;
using System.Collections.Generic;
using Tavis.IANA;


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


		internal HalDocument(HalResource root) : this()
		{
			Root = root;
		}
		
		

        public HalDocument(SelfLink selfLink = null, params HalNode[] contents)
        {
            Namespaces = new List<HalNamespace>();
            Root = new HalResource("self", selfLink == null ? "":selfLink.Target.OriginalString);
            foreach (var content in contents)
            {
                Root.Contents.Add(content.Key,content);
            }
        }


	}
}
