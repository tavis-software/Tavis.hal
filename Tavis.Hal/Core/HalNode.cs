

using System.Collections.Generic;

namespace Tavis {

    public abstract class HalNode {

        protected readonly List<HalNode> _Contents;

        public HalNode Parent { get; internal set; }
		public abstract string Key { get; }

        protected HalNode()
        {
            _Contents = new List<HalNode>();
        }
    }
}