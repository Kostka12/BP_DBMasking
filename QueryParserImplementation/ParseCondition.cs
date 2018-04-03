using ParsedQueryStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParserImplementation
{
    public static class ParseCondition
    {
        public static string[] Operators = new string[] { "like'", "<", ">", "=", "<=", ">=" };
        public static Condition Parse(string aCondition)
        {
            Condition lCondition = new Condition();
            foreach (var nOperator in Operators)
            {
                if (aCondition.Contains(nOperator))
                {
                    lCondition.Operator = CleanOperator(nOperator); 
                    string[] lConditionParts = aCondition.Split(new string[] { nOperator }, StringSplitOptions.RemoveEmptyEntries);
                    lCondition.Aggregate = NotifyAggregateFunction(lConditionParts[0]);
                    lCondition.ColumnName = CleanColumnName(lConditionParts[0].Trim());
                    lCondition.Value = lConditionParts[1].Trim();
                }
            }

            return lCondition;
        }
        public static Tuple<string, string> SplitAlias(string aColName)
        {
            if (aColName.Contains("."))
            {
                string[] lParts = aColName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                return new Tuple<string, string>(lParts[0], lParts[1]);
            }
            else return null;
        }
        public static string CleanColumnName(string aColName)
        {
            string[] lKeywords = { "count(" ,"sum(","avg(","min(","max("};
            string lNewColName = aColName;
            foreach (string nKeyword in lKeywords)
            {
                if (aColName.Contains(nKeyword))
                {
                    lNewColName = (aColName.Replace(nKeyword, String.Empty)).Trim(new char[] { ')' }); 
                }
            }

            return lNewColName;
        }
        public static string CleanOperator(string aOperator)
        {
            string lNewOperator = aOperator;
            if (aOperator.Contains("'"))
            {
                lNewOperator = aOperator.Trim(new char[] { '\'' });
            }
            return lNewOperator;
        }
        public static FunctionEnum NotifyAggregateFunction(string aAggregateCondition)
        {
            string[] lKeywords = { "count(", "avg(", "min(", "max(", "sum(" };
            foreach (string nKeyword in lKeywords)
            {
                if (aAggregateCondition.Contains(nKeyword))
                {
                    switch (nKeyword)
                    {
                        case "count(":
                            return FunctionEnum.COUNT;
                        case "avg(":
                            return FunctionEnum.AVG;
                        case "min(":
                            return FunctionEnum.MIN;
                        case "max(":
                            return FunctionEnum.MAX;
                        case "sum(":
                            return FunctionEnum.SUM;
                        default:
                            return FunctionEnum.NONE;

                    }
                }

            }
            return FunctionEnum.NONE;
        }

        public static List<string> OperatorConvertion(string[] aData,string aOperator, FunctionEnum aFunction, string aValue)
        {
            int lIntVal = 0;
            int lSelectedData;
            List<string> lConditionValues = new List<string>();
            if (!Int32.TryParse(aValue, out lIntVal))
                return lConditionValues;
            if ((aFunction == FunctionEnum.MAX || aFunction == FunctionEnum.MIN) && !Int32.TryParse(aData[0], out lSelectedData))
                return lConditionValues;
                switch (aOperator)
                {
                    case "<":
                    if (aFunction == FunctionEnum.COUNT)
                    {
                        lConditionValues = aData.GroupBy(aR => aR).Where(aR => aR.Count() < lIntVal).Select(aR => aR.First()).ToList();
                    }
                    break;
                    case ">":
                        if (aFunction == FunctionEnum.COUNT)
                        {
                            lConditionValues = aData.GroupBy(aR => aR).Where(aR => aR.Count() > lIntVal).Select(aR => aR.First()).ToList();
                        }
                        //else if (aFunction == FunctionEnum.MIN)
                        //{
                        //    lConditionValues = aData.GroupBy(aR => aR).Where(aR => Int32.Parse(aR.Min()) > lIntVal).ToArray();
                        //}
                        //else if (aFunction == FunctionEnum.MAX)
                        //{
                        //    lConditionValues = aData.GroupBy(aR => aR).Where(aR => Int32.Parse(aR.Min()) > lIntVal).ToArray();
                        //}
                        break;
                    case "=":
                    if (aFunction == FunctionEnum.COUNT)
                    {
                        lConditionValues = aData.GroupBy(aR => aR).Where(aR => aR.Count() == lIntVal).Select(aR => aR.First()).ToList();
                    }
                    break;
                    case "<=":
                    if (aFunction == FunctionEnum.COUNT)
                    {
                        lConditionValues = aData.GroupBy(aR => aR).Where(aR => aR.Count() <= lIntVal).Select(aR => aR.First()).ToList();
                    }
                    break;
                    case ">=":
                    if (aFunction == FunctionEnum.COUNT)
                    {
                        lConditionValues = aData.GroupBy(aR => aR).Where(aR => aR.Count() >= lIntVal).Select(aR => aR.First()).ToList();
                    }
                    break;
                    
                }
            return lConditionValues;
       
        }
    }
}
