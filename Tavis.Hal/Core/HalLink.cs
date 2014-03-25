namespace Tavis {
    internal class HalLink : HalNode {

        private readonly Link _link;
  
        public HalLink(Link link)
        {
            _link = link;
    
        }

        public string Rel
        {
            get { return Link.Relation; }
        }

        public string Href
        {
            get
            {
                if (Link.Target == null) return null;
                return Link.Target.OriginalString;
            }
        }

        public override string Key {
			get { return Link.Relation; }
		}

        public Link Link
        {
            get { return _link; }
        }
    }
}