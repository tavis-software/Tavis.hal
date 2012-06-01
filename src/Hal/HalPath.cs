using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hal {
    public class HalPath {
        private Regex _PathRegex = new Regex(@"/?(\w+)(?:\[(\w+)\])?");
        //private Regex _PathRegex = new Regex(@"/?([\w\:\/]+)(?:\[(\w+)\])?");
        private readonly List<Segment> _Segments = new List<Segment>();
        
		public IEnumerable<Segment> Segments { get { return _Segments; } }


        public HalPath(string path) {
			if (string.IsNullOrWhiteSpace(path)) {
				throw new HalPathException("Path cannot be blank");
			}

            var matches = _PathRegex.Matches(path);
            foreach (Match match in matches) {
				var name = match.Groups[2].Value;
                var segment = new Segment { Key = match.Groups[1].Value + (string.IsNullOrEmpty(name) ? "" : "[" + name + "]") };
                _Segments.Add(segment);
            }
        }


        public class Segment {
			public string Key { get; internal set; }
        }
    }


	public class HalPathException : Exception
	{
		public HalPathException(string message = "Invalid HAL path")
			: base(message)
		{	
		}
	}
}