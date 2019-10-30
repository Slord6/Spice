using Spice.Exceptions;
using Spice.Output;
using System;

namespace Spice
{
    class Program
    {
        static void Main(string[] args)
        {

            string progPath = args.Length > 0 ? args[0] : Console.ReadLine();
            OutputLevel outLevel = args.Length > 1 ? (OutputLevel)Convert.ToInt32(args[1]) : OutputLevel.ERROR;

            ConsoleWriter.OutputLevel = outLevel;
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
