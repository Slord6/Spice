using System;

namespace Spice
{
    class Program
    {
        static void Main(string[] args)
        {
            string progPath = args.Length > 0 ? args[0] : Console.ReadLine();
            Interpreter interp = new Interpreter(progPath);
            interp.Run();
        }
    }
}
