using Spice.Exceptions;
using Spice.Output;
using System;

namespace Spice
{
    class Program
    {
        static void Main(string[] args)
        {
            string progPath;
            if (args.Length > 0)
            {
                progPath = args[0];
            }
            else
            {
                Console.WriteLine("Source path:");
                Console.Write(">");
                progPath = Console.ReadLine();
            }
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
