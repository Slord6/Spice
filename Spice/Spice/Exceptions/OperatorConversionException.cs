using System;
using System.Collections.Generic;
using System.Text;

namespace Spice.Exceptions
{
    class OperatorConversionException : SpiceException
    {
        public OperatorConversionException(string message) : base(message)
        {
        }
    }
}
