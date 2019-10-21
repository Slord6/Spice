using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Spice
{
    class Interpreter
    {
        public Interpreter(string programPath)
        {
            List<Token> tokens = Lexer.Lex(File.ReadAllText(programPath));
            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        public void Run()
        {

        }
    }
}
