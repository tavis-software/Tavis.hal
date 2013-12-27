namespace Tavis {
    public class HalLink : HalNode {
        private readonly Link _link;
        public virtual string Rel { get; set; }
        public string Href { get; set; }

        public HalLink()
        {
            
        }
        public HalLink(Link link)
        {
            _link = link;
            Rel = _link.Relation;
            Href = _link.Target.OriginalString;
        }

        public override string Key {
			get { return Rel; }
		}
    }
}