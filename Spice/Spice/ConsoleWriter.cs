using System;
using System.Collections.Generic;
using System.Text;

namespace Spice
{
    class ConsoleWriter
    {
        public static bool isDebug = false;

        public static void WriteLine(string value)
        {
            if(isDebug)
            {
                Console.WriteLine(value);
            }
        }

        public static void Write(string value)
        {
            if(isDebug)
            {
                Console.Write(value);
            }
        }
    }
}
