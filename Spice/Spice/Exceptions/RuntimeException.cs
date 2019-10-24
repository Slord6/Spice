﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Spice.Exceptions
{
    class RuntimeException : Exception
    {
        public RuntimeException(string message) : base(message)
        {
        }

        public RuntimeException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
