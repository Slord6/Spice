using Spice.Exceptions;
using Spice.Tree;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spice
{
    class ProgramContext
    {
        private Operators operators;
        private Memory memory;
        private int programCounter;
        private SyntaxTree program;

        public Operators Operators
        {
            get
            {
                return operators;
            }
        }

        public int ProgramCounter
        {
            get
            {
                return programCounter;
            }
            set
            {
                programCounter = value;
            }
        }

        public int ProgramLength
        {
            get
            {
                return program.Tree.Count;
            }
        }

        public Memory Memory
        {
            get
            {
                return memory;
            }
        }

        public ProgramContext(Operators operators, SyntaxTree program)
        {
            this.operators = operators;
            this.program = program;
            this.memory = new Memory();
        }

        public Tree<Token> GetNextInstruction()
        {
            programCounter++;
            return (programCounter >= program.Tree.Count) ? null : program.Tree[programCounter];
        }

        public int LineToTokenNumber(int line)
        {
            bool pastSplit = false;
            int tokenCount = 0;
            int opCount = 0;
            foreach (Tree<Token> token in program.Tree)
            {
                tokenCount++;
                TokenType tokenType = token.Root.Value.TokenType;
                if (!pastSplit)
                {
                    if(tokenType == TokenType.ProgramSplitter)
                    {
                        pastSplit = true;
                    }
                    continue;
                }

                if (tokenType == TokenType.Operator || tokenType == TokenType.Unknown) opCount++;
                if (opCount == line) return tokenCount;
            }
            throw new RuntimeException("Invalid line number " + line + ". Context: " + this);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("Context:");
            builder.Append(Environment.NewLine);
            builder.Append(operators.ToString());
            builder.Append(Environment.NewLine);
            builder.Append(memory.ToString());
            return builder.ToString();
        }
    }
}
