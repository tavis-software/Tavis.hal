using System.Xml.Linq;


namespace Hal {
    public class HalProperty : HalNode {
        public XElement Value { get; set; }

		public override string Key {
			get { return Value.Name.LocalName; }
		}
    }
}