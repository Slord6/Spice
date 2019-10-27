using System;
using System.Collections.Generic;
using System.Text;

namespace Spice.Exceptions
{
    class ModuleLoadException : SpiceException
    {
        public ModuleLoadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
