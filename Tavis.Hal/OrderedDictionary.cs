using System.Collections.Generic;
using System.Linq;

namespace Tavis
{

    // need to be ordered
	public class OrderedDictionary<TKey, TValue> : Dictionary<TKey,TValue>
	{
		public TValue this[TKey key] {
			 get { return (TValue)base[key]; }
			 set { base[key] = value; }
		}


		public void Add(TKey key, TValue value)
		{
			base.Add(key, value);
		}


        //public bool ContainsKey(TKey key)
        //{
        //    return base.Contains(key);
        //}


		public new IEnumerable<TValue> Values {
			get { return base.Values.Cast<TValue>(); }
		}
	}
}
