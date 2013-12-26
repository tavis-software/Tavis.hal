using System.Collections;
using System.Linq;

namespace Tavis
{
    public class PropertyFinder
    {
        private readonly HalResource _root;

        public PropertyFinder(HalResource root)
        {
            _root = root;
        }

        public string this[string propertyNameAndPath]
        {
            get {
                var parts = propertyNameAndPath.Split('/');
                var propertyName = parts[0];
                var prop = _root.FindProperty(propertyName);
                
                if (parts.Length > 1 )
                {
                    return prop.GetValue(parts[1]).ToString();
                    
                } else {
                    return prop.GetValue().ToString();    
                    
                }
                
            }
        }
    }
}