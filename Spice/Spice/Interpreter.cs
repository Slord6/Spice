using Spice.Exceptions;
using Spice.Output;
using Spice.Tree;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Spice
{
    class Interpreter
    {
        private SyntaxTree syntaxTree;
        private Operators operators;
        ProgramContext programContext;

        public ProgramContext ProgramContext
        {
            get
            {
                return programContext;
            }
        }

        public Interpreter(string programPath, Operators operators = null)
        {
            if(operators == null)
            {
                operators = Operators.GetInstance(Operators.New());
            }
            this.operators = operators;

            string source;
            try
            {
                source = File.ReadAllText(programPath);
            }
            catch (Exception ex)
            {
                throw new ModuleLoadException("Could not load module " + programPath, ex);
            }

            List<Token> tokens = Lexer.Lex(source, operators);
            syntaxTree = new SyntaxTree(tokens);
            programContext = new ProgramContext(operators, syntaxTree);
        }

        public void Run()
        {
            ConsoleWriter.WriteLine("New interpreter run " + programContext);
            Tree<Token> instruction = programContext.GetNextInstruction();
            while (instruction != null)
            {
                TokenType tokenType = instruction.Root.Value.TokenType;
                switch (tokenType)
                {
                    case TokenType.Variable:
                        programContext.Memory.Declare(instruction.Root.Value.Lexeme);
                        break;
                    case TokenType.Delimiter:
                    case TokenType.ProgramSplitter:
                        break;
                    case TokenType.Unknown:
                        // Done at runtime rather than in Lexer to allow for ALS
                        if (programContext.Operators.IsOperator(instruction.Root.Value.Lexeme))
                        {
                            instruction.Root.Value.TokenType = TokenType.Operator;
                        }
                        RunCommand(instruction, programContext);
                        break;
                    case TokenType.Operator:
                        RunCommand(instruction, programContext);
                        break;
                    case TokenType.Value:
                    default:
                        throw new RuntimeException("Invalid root token: " + instruction.Root.Value + ". At instruction - " + instruction.ToString() + ". Context - " + programContext.ToString());
                }

                instruction = programContext.GetNextInstruction();
            } 
        }

        public ProgramContext RunCommand(Tree<Token> instruction, ProgramContext context)
        {
            ConsoleWriter.WriteLine(Environment.NewLine + "Instruction: " + instruction);
            ConsoleWriter.WriteLine(context.ToString());

            Ops op = context.Operators.ToOp(instruction.Root.Value.Lexeme);
            ConsoleWriter.WriteLine("Run: " + instruction);
            switch (op)
            {
                case Ops.ADD:
                    Add(context, instruction.Root.Children[0].Value.Lexeme, instruction.Root.Children[1].Value.Lexeme, instruction.Root.Children[2].Value.Lexeme);
                    break;
                case Ops.SUB:
                    Sub(context, instruction.Root.Children[0].Value.Lexeme, instruction.Root.Children[1].Value.Lexeme, instruction.Root.Children[2].Value.Lexeme);
                    break;
                case Ops.MUL:
                    Mul(context, instruction.Root.Children[0].Value.Lexeme, instruction.Root.Children[1].Value.Lexeme, instruction.Root.Children[2].Value.Lexeme);
                    break;
                case Ops.DIV:
                    Div(context, instruction.Root.Children[0].Value.Lexeme, instruction.Root.Children[1].Value.Lexeme, instruction.Root.Children[2].Value.Lexeme);
                    break;
                case Ops.MOD:
                    Mod(context, instruction.Root.Children[0].Value.Lexeme, instruction.Root.Children[1].Value.Lexeme, instruction.Root.Children[2].Value.Lexeme);
                    break;
                case Ops.PUT:
                    Put(context, instruction.Root.Children[0].Value.Lexeme, instruction.Root.Children[1].Value.Lexeme, instruction.Root.Children[2].Value.Lexeme);
                    break;
                case Ops.GET:
                    Get(context, instruction.Root.Children[0].Value.Lexeme, instruction.Root.Children[1].Value.Lexeme, instruction.Root.Children[2].Value.Lexeme);
                    break;
                case Ops.SWI:
                    Swi(context, instruction.Root.Children[0].Value.Lexeme, instruction.Root.Children[1].Value.Lexeme, instruction.Root.Children[2].Value.Lexeme);
                    break;
                case Ops.BRK:
                    Brk(context, instruction.Root.Children[0].Value.Lexeme, instruction.Root.Children[1].Value.Lexeme, instruction.Root.Children[2].Value.Lexeme);
                    break;
                case Ops.ALS:
                    AddAlias(context, instruction);
                    break;
                case Ops.OUT:
                    Out(context, instruction);
                    break;
                case Ops.LOD:
                    Lod(context, instruction);
                    break;
                case Ops.SIN:
                    Sin(context, instruction.Root.Children[0].Value.Lexeme, instruction.Root.Children[1].Value.Lexeme);
                    break;
                case Ops.COS:
                    Cos(context, instruction.Root.Children[0].Value.Lexeme, instruction.Root.Children[1].Value.Lexeme);
                    break;
                case Ops.TAN:
                    Tan(context, instruction.Root.Children[0].Value.Lexeme, instruction.Root.Children[1].Value.Lexeme);
                    break;
                case Ops.POW:
                    Pow(context, instruction.Root.Children[0].Value.Lexeme, instruction.Root.Children[1].Value.Lexeme, instruction.Root.Children[2].Value.Lexeme);
                    break;
                case Ops.REA:
                    Rea(context, instruction.Root.Children[0].Value.Lexeme);
                    break;
                case Ops.CLR:
                    Clr(context, instruction.Root.Children[0].Value.Lexeme);
                    break;
                case Ops.LEN:
                    Len(context, instruction.Root.Children[0].Value.Lexeme, instruction.Root.Children[1].Value.Lexeme);
                    break;
                case Ops.NUL:
                    break;
                default:
                    throw new NotImplementedException("Op " + op + " has not been implemented in the interpreter");
            }
            return context;
        }

        private void AddAlias(ProgramContext context, Tree<Token> instruction)
        {
            context.Operators.AddAlias(instruction.Root.Children[0].Value.Lexeme, instruction.Root.Children[1].Value.Lexeme);
        }

        private void Add(ProgramContext context, string val1, string val2, string varName)
        {
            double first = context.Memory.ResolveToSingleValue(val1);
            double second = context.Memory.ResolveToSingleValue(val2);
            context.Memory.SetVarValue(varName, first + second);
        }

        private void Sub(ProgramContext context, string val1, string val2, string varName)
        {
            double first = context.Memory.ResolveToSingleValue(val1);
            double second = context.Memory.ResolveToSingleValue(val2);
            context.Memory.SetVarValue(varName, first - second);
        }

        private void Div(ProgramContext context, string val1, string val2, string varName)
        {
            double first = context.Memory.ResolveToSingleValue(val1);
            double second = context.Memory.ResolveToSingleValue(val2);
            context.Memory.SetVarValue(varName, first / second);
        }

        private void Mul(ProgramContext context, string val1, string val2, string varName)
        {
            double first = context.Memory.ResolveToSingleValue(val1);
            double second = context.Memory.ResolveToSingleValue(val2);
            context.Memory.SetVarValue(varName, first * second);
        }

        private void Mod(ProgramContext context, string val1, string val2, string varName)
        {
            double first = context.Memory.ResolveToSingleValue(val1);
            double second = context.Memory.ResolveToSingleValue(val2);
            context.Memory.SetVarValue(varName, first % second);
        }

        private void Swi(ProgramContext context, string val1, string val2, string val3)
        {
            double first = context.Memory.ResolveToSingleValue(val1);
            double second = context.Memory.ResolveToSingleValue(val2);
            int lineNo = (int)context.Memory.ResolveToSingleValue(val3);
            ConsoleWriter.Write("SWI, PC at " + context.ProgramCounter + " before and ", OutputLevel.DEBUG);
            if (first < second) context.ProgramCounter = context.LineToTokenNumber(lineNo - 1); // -1 as will increment when next instruction requested
            ConsoleWriter.WriteLine(context.ProgramCounter + " after", OutputLevel.DEBUG);
        }

        private void Put(ProgramContext context, string index, string array, string value)
        {
            context.Memory.SetVarValue(array, context.Memory.ResolveToSingleValue(value), (int)context.Memory.ResolveToSingleValue(index));
        }

        private void Get(ProgramContext context, string index, string array, string var)
        {
            double value = context.Memory.GetFromArray(array, (int)context.Memory.ResolveToSingleValue(index));
            context.Memory.SetVarValue(var, value);
        }

        private void Brk(ProgramContext context, string val1, string val2, string varName)
        {
            double first = context.Memory.ResolveToSingleValue(val1);
            double second = context.Memory.ResolveToSingleValue(val2);

            if (first < second)
            {
                context.ProgramCounter = context.ProgramLength; //End prog
                return;
            }
            else
            {
                context.Memory.SetVarValue(varName, Math.Abs(first - second));
            }
        }

        private void Lod(ProgramContext context, Tree<Token> instruction)
        {
            string fileName = instruction.Root.Children[0].Value.Lexeme;
            string stdPrefix = "std::";
            if(fileName.Contains(stdPrefix))
            {
                string execuatbleLocation = new Uri(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)).LocalPath;
                fileName = execuatbleLocation + @"\std\" + fileName.Split(stdPrefix, StringSplitOptions.RemoveEmptyEntries)[0];
            }
            Interpreter moduleInterpreter;
            try
            {
                moduleInterpreter = new Interpreter(fileName);

                Node<Token> passedValToken = instruction.Root.Children[1];
                if (passedValToken.Value.TokenType == TokenType.PassableArray)
                {
                    string varName = passedValToken.Value.Lexeme.Split(Lexer.passableArrayModifier)[1];
                    // In this case, pass the entire array into the first variable of the loaded module
                    moduleInterpreter.ProgramContext.Memory.SetPassedValue(context.Memory.ResolveToValue(varName));
                    Console.WriteLine("VarName = " + varName);
                }
                else
                {
                    // In this case, each element of the value into each declared variable in turn
                    moduleInterpreter.ProgramContext.Memory.SetPassedValues(context.Memory.ResolveToValue(passedValToken.Value.Lexeme));
                    Console.WriteLine("Lexeme: " + passedValToken.Value.Lexeme);
                }
            }
            catch(Exception ex)
            {
                throw new ModuleLoadException("Could not resolve module " + fileName + ". With context - " + context.ToString(), ex);
            }

            try
            {
                moduleInterpreter.Run();
                context.Memory.SetVarValue(instruction.Root.Children[2].Value.Lexeme, moduleInterpreter.ProgramContext.Memory.GetFullValue("return"));
            }
            catch(Exception ex)
            {
                throw new RuntimeException("Module " + fileName + " caused exception with context - " + moduleInterpreter.ProgramContext.ToString(), ex);
            }
        }

        private void Out(ProgramContext context, Tree<Token> instruction)
        {
            string[] output = instruction.Root.Children.Where(n => n.Value.TokenType != TokenType.Delimiter)
                .Select(n => n.Value.TokenType == TokenType.StringLiteral ? n.Value.Lexeme : "[" + string.Join(", ", context.Memory.ResolveToValue(n.Value.Lexeme)) + "]").ToArray();
            ConsoleWriter.WriteLine(String.Join(' ', output), OutputLevel.PROGRAM);
        }

        private void Sin(ProgramContext context, string val, string storeIn)
        {
            double value = context.Memory.ResolveToSingleValue(val);
            context.Memory.SetVarValue(storeIn, new List<double>() { Math.Sin(value) });
        }

        private void Cos(ProgramContext context, string val, string storeIn)
        {
            double value = context.Memory.ResolveToSingleValue(val);
            context.Memory.SetVarValue(storeIn, new List<double>() { Math.Cos(value) });
        }

        private void Tan(ProgramContext context, string val, string storeIn)
        {
            double value = context.Memory.ResolveToSingleValue(val);
            context.Memory.SetVarValue(storeIn, new List<double>() { Math.Tan(value) });
        }

        private void Pow(ProgramContext context, string baseVal, string exponent, string storeIn)
        {
            double value1 = context.Memory.ResolveToSingleValue(baseVal);
            double value2 = context.Memory.ResolveToSingleValue(exponent);
            context.Memory.SetVarValue(storeIn, new List<double> { Math.Pow(value1, value2) });
        }

        private void Rea(ProgramContext context, string storeIn)
        {
            ConsoleWriter.Write(">", OutputLevel.PROGRAM);
            string input = Console.ReadLine();
            List<double> values;
            try
            {
                values = input.Split(", ", StringSplitOptions.RemoveEmptyEntries).Select(x =>
                {
                    return input[0] == '#'
                    ? BitConverter.Int64BitsToDouble(Convert.ToInt64(input.Split('#', StringSplitOptions.RemoveEmptyEntries)[0], 16))
                    : double.Parse(x);
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new RuntimeException("Could not read values from console in: " + input + "." + context, ex);
            }

            context.Memory.SetVarValue(storeIn, values);
        }

        private void Clr(ProgramContext context, string varName)
        {
            context.Memory.Clear(varName);
        }

        private void Len(ProgramContext context, string valOrVar, string var)
        {
            List<double> value = context.Memory.ResolveToValue(valOrVar);
            context.Memory.SetVarValue(var, value.Count);
        }

        private IEnumerable<Node<Token>> InstructionParametersOfTypesFilter(Tree<Token> instruction, TokenType[] types)
        {
            return instruction.Root.Children.Where(n => types.Contains(n.Value.TokenType));
        }
    }
}
