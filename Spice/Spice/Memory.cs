using Spice.Exceptions;
using Spice.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spice
{
    class Memory
    {
        private Dictionary<string, List<double>> memory;
        private Queue<double> passedValues;
        public Memory()
        {
            memory = new Dictionary<string, List<double>>();
            passedValues = new Queue<double>();
        }

        public Dictionary<string, List<double>> Dump()
        {
            return memory;
        }

        public List<double> GetVarValue(string varName, int at = 0)
        {
            try
            {
                return memory[varName];
            }
            catch (Exception ex)
            {
                throw new RuntimeException("Could not access value @ " + varName, ex);
            }
        }

        public List<double> GetFullValue(string varName)
        {
            try
            {
                return memory[varName];
            }
            catch (Exception ex)
            {
                throw new RuntimeException("Could not access value @ " + varName, ex);
            }
        }

        public void SetVarValue(string varName, double value, int at = 0)
        {
            try
            {
                memory[varName].Insert(at, value);
            }
            catch (Exception ex)
            {
                throw new RuntimeException("Could not set value @ " + varName, ex);
            }
        }

        public void SetVarValue(string varName, List<double> value)
        {
            try
            {
                memory[varName] = value;
            }
            catch (Exception ex)
            {
                throw new RuntimeException("Could not set value @ " + varName, ex);
            }
        }

        public void SetPassedValues(List<double> values)
        {
            foreach (double value in values)
            {
                passedValues.Enqueue(value);
            }
        }

        public void Declare(string varName)
        {
            memory.Add(varName, new List<double>());
            if(passedValues.Count > 0)
            {
                memory[varName].Add(passedValues.Dequeue());
                ConsoleWriter.WriteLine("Set " + varName + " to passed value of " + memory[varName][0]);
            }
        }

        public bool IsArray(string varOrVal)
        {
            List<double> value = ResolveToValue(varOrVal);
            return value.Count > 1;
        }

        public double ResolveToSingleValue(string varOrVal)
        {
            List<double> value = ResolveToValue(varOrVal);
            if (value.Count > 0) return value[0];
            return 0;
        }

        public double GetFromArray(string var, int at)
        {
            return ResolveToValue(var)[at];
        }

        public List<double> ResolveToValue(string varOrVal)
        {
            List<double> val = new List<double>();
            double temp = 0;
            bool isValue = double.TryParse(varOrVal, out temp);
            if (!isValue)
            {
                val = GetVarValue(varOrVal);
            }
            else
            {
                val.Add(temp);
            }

            return val;
        }

        public void Clear(string varName)
        {
            if(!memory.ContainsKey(varName)) throw new RuntimeException("Could not clear value @ " + varName);
            memory[varName] = new List<double>();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("Memory: ");
            foreach (KeyValuePair<string,List<double>> variable in memory)
            {
                builder.Append(variable.Key);
                builder.Append("|[");
                builder.Append(String.Join(", ", variable.Value.Select(variable => variable.ToString())));
                builder.Append("] ");
            }
            return builder.ToString();
        }
    }
}
