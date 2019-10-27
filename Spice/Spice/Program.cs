using Spice.Exceptions;
using Spice.Output;
using System;

namespace Spice
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleWriter.OutputLevel = OutputLevel.ERROR;

            string progPath = args.Length > 0 ? args[0] : Console.ReadLine();
            try
            {
                Interpreter interp = new Interpreter(progPath);
                interp.Run();
            }
            catch (Exception ex)
            {
                bool includeStack = ex.GetType() == typeof(SpiceException);
                ConsoleWriter.Write(ex, includeStack);
            }
        }
    }
}
