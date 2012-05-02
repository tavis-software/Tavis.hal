using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hal
{
	public interface IHalResource
	{
		string Rel { get; set; }
		string Name { get; set; }
		string Href { get; set; }
		string Type { get; set; }

		OrderedDictionary<string, HalNode> Contents { get; }

		string Key { get; }

		IHalResource Parent { get; }
	}
}
