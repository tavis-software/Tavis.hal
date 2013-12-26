using System.IO;

namespace Tavis
{
    public class JsonHalReader : IHalReader
    {
  
        public HalDocument Load(Stream xmlStream)
        {
         return  new HalDocument(new HalResource("",""));
        }
    }
}