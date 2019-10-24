using Spice.Exceptions;
using Spice.Tree;
using System;
using System.Collections.Generic;
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


            List<Token> tokens = Lexer.Lex(File.ReadAllText(programPath), operators);

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
                        programContext.Memory.Declare(instruction.Root.Value.RawValue);
                        break;
                    case TokenType.Delimiter:
                    case TokenType.ProgramSplitter:
                        break;
                    case TokenType.Unknown:
                        // Done at runtime rather than in Lexer to allow for ALS
                        if (programContext.Operators.IsOperator(instruction.Root.Value.RawValue))
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

            Ops op = context.Operators.ToOp(instruction.Root.Value.RawValue);
            ConsoleWriter.WriteLine("Run: " + instruction);
            switch (op)
            {
                case Ops.ADD:
                    Add(context, instruction.Root.Children[0].Value.RawValue, instruction.Root.Children[1].Value.RawValue, instruction.Root.Children[2].Value.RawValue);
                    break;
                case Ops.SUB:
                    Sub(context, instruction.Root.Children[0].Value.RawValue, instruction.Root.Children[1].Value.RawValue, instruction.Root.Children[2].Value.RawValue);
                    break;
                case Ops.MUL:
                    Mul(context, instruction.Root.Children[0].Value.RawValue, instruction.Root.Children[1].Value.RawValue, instruction.Root.Children[2].Value.RawValue);
                    break;
                case Ops.DIV:
                    Div(context, instruction.Root.Children[0].Value.RawValue, instruction.Root.Children[1].Value.RawValue, instruction.Root.Children[2].Value.RawValue);
                    break;
                case Ops.MOD:
                    Mod(context, instruction.Root.Children[0].Value.RawValue, instruction.Root.Children[1].Value.RawValue, instruction.Root.Children[2].Value.RawValue);
                    break;
                case Ops.PUT:
                    Put(context, instruction.Root.Children[0].Value.RawValue, instruction.Root.Children[1].Value.RawValue, instruction.Root.Children[2].Value.RawValue);
                    break;
                case Ops.GET:
                    Get(context, instruction.Root.Children[0].Value.RawValue, instruction.Root.Children[1].Value.RawValue, instruction.Root.Children[2].Value.RawValue);
                    break;
                case Ops.SWI:
                    Swi(context, instruction.Root.Children[0].Value.RawValue, instruction.Root.Children[1].Value.RawValue, instruction.Root.Children[2].Value.RawValue);
                    break;
                case Ops.BRK:
                    Brk(context, instruction.Root.Children[0].Value.RawValue, instruction.Root.Children[1].Value.RawValue, instruction.Root.Children[2].Value.RawValue);
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
                case Ops.NUL:
                default:
                    break;
            }
            return context;
        }

        private void AddAlias(ProgramContext context, Tree<Token> instruction)
        {
            context.Operators.AddAlias(instruction.Root.Children[0].Value.RawValue, instruction.Root.Children[1].Value.RawValue);
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
            if (first < second) context.ProgramCounter = context.LineToTokenNumber(lineNo - 1); // -1 as will increment when next instruction requested
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
            string fileName = instruction.Root.Children[0].Value.RawValue;
            Interpreter moduleInterpreter;
            try
            {
                moduleInterpreter = new Interpreter(fileName);

                // This doesn't work because no variables exist yet, could push to stack and then pop as declared?
                moduleInterpreter.ProgramContext.Memory.SetPassedValues(context.Memory.ResolveToValue(instruction.Root.Children[1].Value.RawValue)); // Load module with passed values
            }
            catch(Exception ex)
            {
                throw new RuntimeException("Could not resolve module " + fileName + ". With context - " + context.ToString(), ex);
            }

            try
            {
                moduleInterpreter.Run();
                context.Memory.SetVarValue(instruction.Root.Children[2].Value.RawValue, moduleInterpreter.ProgramContext.Memory.GetFullValue("return"));
            }
            catch(Exception ex)
            {
                throw new RuntimeException("Module " + fileName + " caused exception with context - " + moduleInterpreter.ProgramContext.ToString(), ex);
            }
        }

        private void Out(ProgramContext context, Tree<Token> instruction)
        {
            string[] output = InstructionParametersOfTypesFilter(instruction, new TokenType[] { TokenType.Value, TokenType.Variable })
                .Select(n => context.Memory.ResolveToSingleValue(n.Value.RawValue).ToString()).ToArray();
            Console.WriteLine(String.Join(' ', output));
        }

        private IEnumerable<Node<Token>> InstructionParametersOfTypesFilter(Tree<Token> instruction, TokenType[] types)
        {
            return instruction.Root.Children.Where(n => types.Contains(n.Value.TokenType));
        }
    }
}
