using Spice.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spice
{
    class Operators
    {
        private static Operators instance = null;
        private Dictionary<string, string> aliases;
        private string[] operators;

        public static Operators GetInstance()
        {
            if (instance == null)
            {
                instance = new Operators();
            }
            return instance;
        }

        private Operators()
        {
            this.operators = new string[]
            {
                "ADD",
                "SUB",
                "MUL",
                "DIV",
                "MOD",
                "PUT",
                "GET",
                "SWI",
                "BRK",
                "ALS",
                "OUT",
                "LOD",
            };
            this.aliases = new Dictionary<string, string>();
        }

        public void AddAlias(string alias, string op)
        {
            this.aliases.Add(alias, op);
        }


        public bool IsOperator(string value)
        {
            for (int i = 0; i < operators.Length; i++)
            {
                if (operators[i] == value.ToUpper()) return true;
            }
            foreach (KeyValuePair<string,string> alias in aliases)
            {
                if (alias.Key == value.ToUpper()) return true;
            }

            return false;
        }

        public bool TryResolveAlias(string alias, out string op)
        {
            return aliases.TryGetValue(alias, out op);
        }
        public Ops ToOp(string op)
        {
            switch (op)
            {
                case "ADD":
                    return Ops.ADD;
                case "SUB":
                    return Ops.SUB;
                case "MUL":
                    return Ops.MUL;
                case "DIV":
                    return Ops.DIV;
                case "MOD":
                    return Ops.MOD;
                case "PUT":
                    return Ops.PUT;
                case "GET":
                    return Ops.GET;
                case "SWI":
                    return Ops.SWI;
                case "BRK":
                    return Ops.BRK;
                case "ALS":
                    return Ops.ALS;
                case "OUT":
                    return Ops.OUT;
                case "LOD":
                    return Ops.LOD;
                default:
                    throw new OperatorConversionException("Op not recognised: " + op);
            }
        }
    }
}
