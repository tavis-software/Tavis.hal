using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Hal;

namespace Tavis
{
    public interface IHalReader
    {
        HalDocument Load(Stream xmlStream);
    }
}
