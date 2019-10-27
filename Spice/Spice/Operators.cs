using Spice.Exceptions;
using Spice.Tree;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spice
{
    class Operators
    {
        private static Dictionary<int, Operators> operatorContexts = new Dictionary<int, Operators>();
        private Dictionary<string, string> aliases;
        private string[] operators;
        private int id;

        public int ID
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// Get an Operators context from its ID
        /// </summary>
        /// <param name="ID">Operators ID</param>
        /// <returns>Operators instance</returns>
        public static Operators GetInstance(int ID)
        {
            return operatorContexts[ID];
        }

        /// <summary>
        /// Create a new Operators context
        /// </summary>
        /// <returns>ID</returns>
        public static int New()
        {
            int ID = operatorContexts.Count;
            operatorContexts.Add(ID, new Operators(ID));
            return ID;
        }

        private Operators(int ID)
        {
            this.id = ID;
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
                "SIN",
                "COS",
                "TAN",
                "POW",
                "REA"
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
            if (aliases.ContainsKey(op)) op = aliases[op];
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
                case "NUL":
                    return Ops.NUL;
                case "SIN":
                    return Ops.SIN;
                case "COS":
                    return Ops.COS;
                case "TAN":
                    return Ops.TAN;
                case "POW":
                    return Ops.POW;
                case "REA":
                    return Ops.REA;
                case "CLR":
                    return Ops.CLR;
                default:
                    throw new OperatorConversionException("Op not recognised: " + op);
            }
        }

        public override string ToString()
        {
            List<string> aliasNames = new List<string>();
            foreach (KeyValuePair<string, string> alias in aliases)
            {
                aliasNames.Add(alias.Key);
            }
            return "Operators: " + String.Join(", ", operators) + " | " + string.Join(", ", aliasNames);
        }
    }
}
