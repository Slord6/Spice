﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Spice.Exceptions
{
    class ParseException : SpiceException
    {
        public ParseException(string message) : base(message)
        {
        }
    }
}
