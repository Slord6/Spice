using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Spice
{
    static class Lexer
    {
        private static char progSplit = '@';

        public static List<Token> Lex(string source)
        {
            List<Token> tokens = new List<Token>();
            char delimiter = GetDelimiter(source);

            string[] splitProg = source.Split(progSplit);

            tokens.AddRange(SectionToTokens(splitProg[0], delimiter));
            tokens.Add(new Token(progSplit.ToString(), TokenType.ProgramSplitter));
            List<Token> variables = ResolveVariables(tokens);
            tokens.AddRange(SectionToTokens(splitProg[1], delimiter, variables));
            return tokens;
        }

        /// <summary>
        /// Updates unknown tokens to be TokenType.Variables
        /// </summary>
        /// <param name="declarationSectionTokens"></param>
        /// <returns>List of updated tokens</returns>
        private static List<Token> ResolveVariables(List<Token> declarationSectionTokens)
        {
            List<Token> updated = new List<Token>();
            foreach (Token token in declarationSectionTokens)
            {
                if (token.TokenType == TokenType.Unknown)
                {
                    token.TokenType = TokenType.Variable;
                    updated.Add(token);
                }
            }
            return updated;
        }

        private static List<Token> SectionToTokens(string section, char delimiter, List<Token> variables = null)
        {
            string[] sectionLines = section.Split(delimiter);
            List<Token> tokens = new List<Token>();
            foreach (string line in sectionLines)
            {
                tokens.AddRange(LineToTokens(line, delimiter, variables));
                tokens.Add(new Token(delimiter.ToString(), TokenType.Delimiter));

            }
            return tokens;
        }

        private static List<Token> LineToTokens(string line, char delimiter, List<Token> variables)
        {
            List<Token> tokens = new List<Token>();
            string[] tokenSplit = line.Split(' ');
            foreach (string token in tokenSplit)
            {
                if (String.IsNullOrWhiteSpace(token) || String.IsNullOrEmpty(token)) continue;
                string cleanToken = token.Replace(Environment.NewLine, "");
                tokens.Add(new Token(cleanToken, GetTokenType(cleanToken, delimiter, variables)));
            }
            return tokens;
        }

        private static TokenType GetTokenType(string token, char delimiter, List<Token> variables)
        {
            if (token[0] == delimiter) return TokenType.Delimiter;

            if (variables != null && variables.Select(t => t.RawValue).Contains(token)) return TokenType.Variable;

            if (Operators.GetInstance().IsOperator(token)) return TokenType.Operator;

            double testDouble;
            if(Double.TryParse(token, out testDouble))
            {
                return TokenType.Value;
            }

            return TokenType.Unknown;
        }

        private static char GetDelimiter(string source)
        {
            return source[0];
        }
    }
}
