using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hal;

namespace Tavis
{
    public class HalLinkExtractor : ILinkExtractor
    {
        public Type SupportedType { get { return typeof (HalDocument); } }


        public Link GetLink(Func<string,Link> factory, object content, string relation, string anchor = null)
        {
            var halContent = (HalDocument)content;
            var halLink = halContent.FindLink(anchor + relation);
            Link link = factory(halLink.Rel);
            link.Context = anchor != null ? new Uri(anchor, UriKind.RelativeOrAbsolute) : null;
            link.Target = new Uri(halLink.Href, UriKind.RelativeOrAbsolute);

            return link;
        }

        public IEnumerable<Link> GetLinks(Func<string, Link> factory, object content, string relation = null, string anchor = null)
        {
            throw new NotImplementedException();
        }
    }
}
