using System;
using System.Collections.Generic;
using System.Text;

namespace Spice.Exceptions
{
    class SpiceException : Exception
    {

        public SpiceException(string message) : base(message)
        {
        }

        public SpiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
