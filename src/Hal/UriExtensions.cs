using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hal
{
	internal static class UriExtensions
	{
		internal static string AsString(this Uri uri)
		{
			if (uri == null) return null;

			return uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString;
		}



		internal static Uri AsUri(this string uri)
		{
			if (uri == null) return null;

			return new Uri(uri, UriKind.RelativeOrAbsolute);
		}
	}
}
