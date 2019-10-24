using System;
using System.Collections.Generic;
using System.Text;

namespace Spice.Exceptions
{
    class ParseException : Exception
    {
        public ParseException(string message) : base(message)
        {
        }
    }
}
