using System;
using System.Collections.Generic;
using System.Text;

namespace Spice.Exceptions
{
    class OperatorConversionException : Exception
    {
        public OperatorConversionException(string message) : base(message)
        {
        }
    }
}
