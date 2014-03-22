using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Tavis {
    public class HalPath {
        
        private Regex _PathRegex = new Regex(@"/?([\w\:\%\.]+)(?:\[(\w+)\])?");
        private readonly List<Segment> _Segments = new List<Segment>();
        
		public IEnumerable<Segment> Segments { get { return _Segments; } }


        public HalPath(string path) {
			if (string.IsNullOrWhiteSpace(path)) {
				throw new HalPathException("Path cannot be blank");
			}

            var matches = _PathRegex.Matches(path);
            foreach (Match match in matches) {
				var name = match.Groups[2].Value;
                var key = match.Groups[1].Value + (string.IsNullOrEmpty(name) ? "" : "[" + name + "]");
                key = Tavis.HeaderEncodingParser.UriDecode(key, Encoding.UTF8);
                var segment = new Segment { Key = key };
                
                _Segments.Add(segment);
            }
        }


        public class Segment {
			public string Key { get; internal set; }
        }
    }
}