using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tavis
{
    public interface IHalWriter
    {
        void CopyToStream(HalDocument document, Stream stream);
        Stream ToStream(HalDocument document);
    }
}
