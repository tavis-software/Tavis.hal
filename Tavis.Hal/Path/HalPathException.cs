using System;

namespace Tavis
{
    public class HalPathException : Exception
    {
        public HalPathException(string message = "Invalid HAL path")
            : base(message)
        {	
        }
    }
}