using System.Xml.Linq;

namespace Tavis
{
	public class HalTypedResourceContents : HalNode
	{
		public XElement ContentNode { get; set; }

		public override string Key {			// Only ever one in a resource
			get { return "Content"; }
		}
	}
}
