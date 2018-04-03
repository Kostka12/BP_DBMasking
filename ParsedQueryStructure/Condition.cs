using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsedQueryStructure
{
    public class Condition
    {
        public string ColumnName { get; set; }
        public string Operator { get; set;}
        public string Value { get; set; }
        public FunctionEnum Aggregate { get; set; }
        public Condition() { }
        public Condition(string aColumnName, string aOperator, string aValue)
        {
            ColumnName = aColumnName;
            Operator = aOperator;
            Value = CleanValue(aValue.ToLower());
        }
        public string CleanValue(string aValue)
        {
            if (aValue.Contains("'"))
            {
                return aValue.Trim(new char[] { (char)39 });
            }
            return aValue;
        }
        public static bool IsSameCondition(Condition aCondition1, Condition aCondition2)
        {
            if (aCondition1.ColumnName == aCondition2.ColumnName
                && aCondition1.Operator == aCondition2.Operator
                && aCondition1.Value == aCondition2.Value
                && aCondition1.Aggregate == aCondition2.Aggregate)
                return true;
            else return false;
        }
    }
}
