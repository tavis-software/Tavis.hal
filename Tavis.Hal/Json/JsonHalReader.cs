using System.IO;
using Tavis.IANA;

namespace Tavis
{
    public class JsonHalReader : IHalReader
    {
  
        public HalDocument Load(Stream xmlStream)
        {
         return  new HalDocument(new HalResource(new SelfLink()));
        }
    }
}