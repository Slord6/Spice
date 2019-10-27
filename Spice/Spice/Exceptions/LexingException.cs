using System;
using System.Collections.Generic;
using System.Text;

namespace Spice.Exceptions
{
    class LexingException : SpiceException
    {
        public LexingException(string message) : base(message)
        {
        }
    }
}
