using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tavis
{
    public interface IHalWriter
    {
        void CopyToStream(HalResource resource, Stream stream);
        Stream ToStream(HalResource resource);
    }
}
