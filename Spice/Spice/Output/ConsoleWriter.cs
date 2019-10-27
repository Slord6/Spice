using System;
using System.Text;

namespace Spice.Output
{
    class ConsoleWriter
    {
        public static OutputLevel OutputLevel = OutputLevel.INFO;

        public static void WriteLine(string value, OutputLevel messageOutputLevel = OutputLevel.DEBUG)
        {
            ConsoleWriter.Write(value + Environment.NewLine, messageOutputLevel);
        }

        public static void Write(string value, OutputLevel messageOutputLevel = OutputLevel.DEBUG)
        {
            if ((int)messageOutputLevel <= (int)ConsoleWriter.OutputLevel)
            {
                Console.Write(value);
            }
        }

        public static void Write(Exception ex, bool includeStack = false)
        {
            StringBuilder builder = new StringBuilder("EXCEPTION" + Environment.NewLine);
            Exception inner = ex;
            while(inner != null)
            {
                builder.Append(inner.Message + Environment.NewLine);
                if(includeStack) builder.Append(inner.StackTrace + Environment.NewLine);
                inner = inner.InnerException;
            }
            ConsoleWriter.WriteLine(builder.ToString(), OutputLevel.ERROR);
        }
    }
}
