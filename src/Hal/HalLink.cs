using System;

namespace Hal {
    public class HalLink : HalNode {
		public virtual string Rel { get; set; }
        public string Href { get; set; }

		public override string Key {
			get { return Rel; }
		}
    }
}