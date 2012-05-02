using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace Hal
{
	public class HalTypedResourceContents : HalNode
	{
		public XElement ContentNode { get; set; }

		public override string Key {			// Only ever one in a resource
			get { return "Content"; }
		}
	}
}
