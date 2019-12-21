using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Spice.Exceptions;

namespace Spice
{
    static class Lexer
    {
        private static char progSplit = '@';
        public static readonly char passableArrayModifier = '^';

        public static List<Token> Lex(string source, Operators operators)
        {
            List<Token> tokens = new List<Token>();

            if (source.Length == 0) return tokens;

            char delimiter = GetDelimiter(source);

            string[] splitProg = source.Split(progSplit);

            if(splitProg.Length > 2)
            {
                string merged = "";
                for (int i = 1; i < splitProg.Length; i++)
                {
                    merged += splitProg[i];
                    if (i != splitProg.Length - 1) merged += progSplit;
                    splitProg[i] = null;
                }
                splitProg[1] = merged;
            }

            tokens.AddRange(SectionToTokens(splitProg[0], delimiter, operators));
            tokens.Add(new Token(progSplit.ToString(), TokenType.ProgramSplitter));
            List<Token> variables = ResolveVariables(tokens);
            tokens.AddRange(SectionToTokens(splitProg[1], delimiter, operators, variables));
            return tokens;
        }

        /// <summary>
        /// Updates unknown tokens to be TokenType.Variables (assumes declaration section)
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

        private static List<Token> SectionToTokens(string section, char delimiter, Operators operators, List<Token> variables = null)
        {
            string[] sectionLines = section.Split(delimiter);
            List<Token> tokens = new List<Token>();
            foreach (string line in sectionLines)
            {
                tokens.AddRange(LineToTokens(line, delimiter, variables, operators));
                tokens.Add(new Token(delimiter.ToString(), TokenType.Delimiter));

            }
            return tokens;
        }

        private static List<Token> LineToTokens(string line, char delimiter, List<Token> variables, Operators operators)
        {
            List<Token> tokens = new List<Token>();
            string[] tokenSplit = line.Split(' ');
            char stringLiteralWrapper = '"';
            StringBuilder stringLiteralBuilder = null;
            for (int i = 0; i < tokenSplit.Length; i++)
            {
                string token = tokenSplit[i];
                if (String.IsNullOrWhiteSpace(token) || String.IsNullOrEmpty(token)) continue;
                string cleanToken = token.Replace(Environment.NewLine, "");

                if(stringLiteralBuilder != null)
                {
                    stringLiteralBuilder.Append(' ');
                    for (int c = 0; c < token.Length; c++)
                    {
                        if (token[c] == stringLiteralWrapper)
                        {
                            if (c != token.Length - 1)
                            {
                                throw new LexingException("Cannot have string literal ending in the middle of a word at " + line + " in " + token);
                            }
                            token = stringLiteralBuilder.ToString();
                            stringLiteralBuilder = null;
                            tokens.Add(new Token(token, TokenType.StringLiteral));
                            break;
                        } else {
                            stringLiteralBuilder.Append(token[c]);
                        }
                    }
                    continue;
                }

                if (token.Contains(stringLiteralWrapper))
                {
                    if(token[0] != stringLiteralWrapper)
                    {
                        throw new LexingException("Cannot have string literal beginning in the middle of a word at " + line + " in " + token);
                    }
                    stringLiteralBuilder = new StringBuilder();
                    if(token.Length > 1)
                    {
                        string[] splitToken = token.Split(stringLiteralWrapper);
                        stringLiteralBuilder.Append(splitToken[1]);
                        if (splitToken.Length == 3)
                        {
                            tokens.Add(new Token(stringLiteralBuilder.ToString(), TokenType.StringLiteral));
                            stringLiteralBuilder = null;
                        }
                    }
                }
                else
                {
                    tokens.Add(new Token(cleanToken, GetTokenType(cleanToken, delimiter, variables, operators)));
                }
            }
            return tokens;
        }

        private static TokenType GetTokenType(string token, char delimiter, List<Token> variables, Operators operators)
        {
            if (token[0] == delimiter) return TokenType.Delimiter;

            if (variables != null) {
                if(variables.Select(t => t.Lexeme).Contains(token)) return TokenType.Variable;
                string[] var = token.Split(passableArrayModifier);
                if (var.Length > 1 && variables.Select(t => t.Lexeme).Contains(var[1])) return TokenType.PassableArray;
            }

            if (operators.IsOperator(token)) return TokenType.Operator;

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
