using System.Xml.Linq;

namespace Tavis
{
	public class HalTypedResourceContents : HalNode
	{
		public XElement ContentNode { get; set; }

	    public HalTypedResourceContents()
	    {
	        
	    }

	    public HalTypedResourceContents(string contents)
	    {
	        ContentNode = new XElement("TypedResourceContentsWrapper", contents);
	    }
		public override string Key {			// Only ever one in a resource
			get { return "Content"; }
		}
	}
}
