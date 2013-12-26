using System.Collections;
using System.Linq;
using System.Xml.Linq;

using Newtonsoft.Json.Linq;

namespace Tavis {

    public class HalXProperty : HalNode, IHalProperty {
        public XElement Content { get; set; }

		public override string Key {
			get { return Content.Name.LocalName; }
		}


        public object GetContent()
        {
            return Content;
        }

        public object GetValue(string path = null)
        {
            var xValue = Content;

            if (path != null)
            {
                var xPath = path;
                
                var result = (object)"";  //TODO xValue.XPathEvaluate(xPath);
                if (result is string)
                {
                    return (string)result;
                }
                else
                    if (result is double)
                    {
                        return ((double)result).ToString();
                    }
                    else
                    {
                        var att = (IEnumerable)result;
                        return att.Cast<XAttribute>().FirstOrDefault().Value;
                    }
            }
            else
            {
                return xValue.Value;

            }
        }
    }

    public class HalJProperty : HalNode, IHalProperty
    {
        public JProperty Content { get; set; }



        public override string Key
        {
            get { return Content.Name; }
        }

        public object GetContent()
        {
            return Content;
        }

        public object GetValue(string path)
        {
            throw new System.NotImplementedException();
        }
    }



    public interface IHalProperty
    {
        object GetContent();
        object GetValue(string path = null);
    }
}