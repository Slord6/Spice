using System;
using System.Collections.Generic;
using System.Text;
using Spice.Tree;

namespace Spice
{
    class SyntaxTree
    {
        List<Tree<Token>> tree;

        public List<Tree<Token>> Tree
        {
            get
            {
                return tree;
            }
        }

        public SyntaxTree(List<Token> tokens)
        {
            BuildTree(tokens);
        }

        private void BuildTree(List<Token> tokens)
        {
            this.tree = new List<Tree<Token>>();
            for(int i = 0; i < tokens.Count; i++)
            {
                int start = i;
                int end = i;
                
                while(!IsOpEnd(tokens[end]))
                {
                    end++;
                }

                tree.Add(BuildOpTree(tokens, start, end));

                i = end;
            }
        }

        private bool IsOpEnd(Token token)
        {
            return token.TokenType == TokenType.Delimiter || token.TokenType == TokenType.ProgramSplitter;
        }

        private Tree<Token> BuildOpTree(List<Token> tokens, int start, int end)
        {
            Tree<Token> tree = new Tree<Token>();
            tree.Root = new Node<Token>(tokens[start]);
            for (int i = start + 1; i <= end; i++)
            {
                tree.Root.Children.Add(new Node<Token>(tokens[i]));
            }
            return tree;
        }
    }
}
