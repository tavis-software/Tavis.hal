using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Hal
{
	public class OrderedDictionary<TKey, TValue> : OrderedDictionary
	{
		public TValue this[TKey key] {
			 get { return (TValue)base[key]; }
			 set { base[key] = value; }
		}


		public void Add(TKey key, TValue value)
		{
			base.Add(key, value);
		}


		public bool ContainsKey(TKey key)
		{
			return base.Contains(key);
		}


		public new IEnumerable<TValue> Values {
			get { return base.Values.Cast<TValue>(); }
		}
	}
}
