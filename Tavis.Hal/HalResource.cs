using System;

namespace Tavis {
    public class HalResource : HalNode, IHalResource {
		public string Rel { get; set; }
        public string Href { get; set; }
        public string Type { get; set; }

        public string Name { get; set; }
        public Link Link { get; set; }
        public IHalResource Parent { get; internal set; }

		public OrderedDictionary<string, HalNode> Contents { get; set; }


        private ResourcesFinder _Resources;
        private PropertyFinder _Properties;

        public HalResource(string rel, Uri href) : this(rel, href.AsString())
    	{
            
    	}

        public HalResource(string rel, string href)
            : this(new Link() { Relation = rel, Target = new Uri(href, UriKind.RelativeOrAbsolute) })
        {
        }

        public HalResource(Link resourceLink)
        {
            Contents = new OrderedDictionary<string, HalNode>();
            _Resources = new ResourcesFinder(this);
            _Properties = new PropertyFinder(this);
            Link = resourceLink;
            Rel = resourceLink.Relation;
            if (resourceLink.Target != null)
            {
                Href = resourceLink.Target.OriginalString;
            }

        }
		

        public override string Key {
            get {
                return Rel + (string.IsNullOrEmpty(Name) ? string.Empty : "[" + Name + "]");
            }
        }


        public PropertyFinder Properties { get { return _Properties; } }
        public ResourcesFinder Resources { get { return _Resources; }}
    }
}
