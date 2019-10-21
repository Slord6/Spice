using System;
using System.Collections.Generic;
using System.Text;

namespace Spice
{
    class Token
    {
        private string token;
        private TokenType type;

        public Token(string token, TokenType type)
        {
            this.token = token;
            this.type = type;
        }

        public string RawValue
        {
            get
            {
                return token;
            }
        }

        public TokenType TokenType
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        public override string ToString()
        {
            return type.ToString() + " " + token;
        }
    }
}