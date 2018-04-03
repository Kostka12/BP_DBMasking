using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mask
{
    public static class Mask
    {
        public static bool StrOperator(string aOperator, string aValue1, string aValue2)
        {
            aOperator = aOperator.ToLower();
            if (aOperator == "=" || aOperator == "like")
            {
                if (aValue1.Equals(aValue2))
                    return true;
                return false;
            }
            return false;
        }
        public static bool DateOperator(string aOperator, DateTime aValue1, DateTime aValue2)
        {
            switch (aOperator)
            {
                case "=":
                    if (aValue1 == aValue2)
                        return true;
                    return false;
                case ">":
                    if (aValue1 > aValue2)
                        return true;
                    return false;
                case "<":
                    if (aValue1 < aValue2)
                        return true;
                    return false;
                case ">=":
                    if (aValue1 >= aValue2)
                        return true;
                    return false;
                case "<=":
                    if (aValue1 <= aValue2)
                        return true;
                    return false;
            }
            return false;
        }
        public static bool Operator(string aOperator, int aValue1, int aValue2)
        {
            switch (aOperator)
            {
                case ">":
                    if (aValue1 > aValue2)
                        return true;
                    return false;
                case "<":
                    if (aValue1 < aValue2)
                        return true;
                    return false;
                case "=":
                    if (aValue1 == aValue2)
                        return true;
                    return false;
                case "<=":
                    if (aValue1 <= aValue2)
                        return true;
                    return false;
                case ">=":
                    if (aValue1 >= aValue2)
                        return true;
                    return false;
            }
            return false;
        }
        public static bool BoolOperator(string aOperator, bool aValue1, bool aValue2)
        {
            switch (aOperator)
            {
                case "like":
                    if (aValue1 == aValue2)
                        return true;
                    return false;
                case "=":
                    if (aValue1 == aValue2)
                        return true;
                    return false;
            }
            return false;
        }
        public static int MaskInt(int aValue, string aFirstParameter, string aSecondParameter, string aFunctionName)
        {
            int lMaskedValue = 0;
            int lFirstParameter = 0;
            int lSecondParameter = 0;
            switch (aFunctionName)
            {
                case "Hash":
                    lMaskedValue = Integer.Hash(aValue);
                    break;
                case "Random Value":
                    if (Int32.TryParse(aFirstParameter, out lFirstParameter))
                        return aValue;
                    if (Int32.TryParse(aSecondParameter, out lSecondParameter))
                        return aValue;
                    lMaskedValue = Integer.RandomValue(aValue);
                    break;
                case "Offset":
                    if (Int32.TryParse(aFirstParameter, out lFirstParameter))
                        return aValue;
                    if (Int32.TryParse(aSecondParameter, out lSecondParameter))
                        return aValue;
                    lMaskedValue = Integer.Offset(aValue, lFirstParameter, lSecondParameter);
                    break;
                case "RangedValue":
                    if (Int32.TryParse(aFirstParameter, out lFirstParameter))
                        return aValue;
                    if (Int32.TryParse(aSecondParameter, out lSecondParameter))
                        return aValue;
                    lMaskedValue = Integer.RangedValue(lFirstParameter, lSecondParameter);
                    break;
            }
            return lMaskedValue;
        }
        public static bool MaskBool(bool aValue, string aFunctionName)
        {
            bool lMaskedValue = false;
            switch (aFunctionName)
            {

                case "Random Value":
                    {
                        lMaskedValue = Bool.RandomValue(aValue);
                    }
                    break;
                default:
                    break;
            }
            return lMaskedValue;
        }
        public static string MaskString(string aValue, string aFirstParameter, string aSecondParameter, string aFunctionName)
        {
            string lMaskedValue = "";
            int lFirstParameter = Int32.Parse(aFirstParameter);
            switch (aFunctionName)
            {
                case "Random String Part":
                    {
                        int lSecondParameter = Int32.Parse(aSecondParameter);
                        lMaskedValue = String.RandStringPart(aValue, lFirstParameter, lSecondParameter);
                    }
                    break;
                case "Replace String Part":
                    {
                        char lSecondParameter = Convert.ToChar(aSecondParameter);
                        lMaskedValue = String.ReplaceStringPart(aValue, lFirstParameter, lSecondParameter);
                    }
                    break;
                //case "Create Email":
                //    lMaskedValue = String.ConcatenateEmail("", "", "");
                    break;
            }
            return lMaskedValue;
        }
        public static DateTime MaskDate(DateTime aValue, string aFunctionName)
        {
            DateTime lMaskedValue = new DateTime();
            switch (aFunctionName)
            {
                case "Hash date":
                    lMaskedValue = Date.Hash(aValue);
                    break;
            }

            return lMaskedValue;
        }

        public static string ApplyMasking(string aValue, string aType, string aFunctionName, string aFirstParameter, string aSecondParameter, List<ParsedQueryStructure.Condition> aConditions = null)
        {
            string lMaskedValue = "";
            bool lShouldMask = false;
            switch (aType)
            {
                case "int":
                    {
                        var lValue = Int32.Parse(aValue);
                        
                        if (aConditions != null)
                        {
                            foreach(var nCondition in aConditions)
                            {
                                if (!Mask.Operator(nCondition.Operator, lValue, Int32.Parse(nCondition.Value)))
                                {
                                    lShouldMask = true;
                                    continue;
                                }
                                else
                                {
                                    lShouldMask = false;
                                    break;
                                }
                            }
                            lMaskedValue = lShouldMask ? Mask.MaskInt(lValue, aFirstParameter, aSecondParameter, aFunctionName).ToString(): lValue.ToString();
                        }
                        else
                        {
                            lMaskedValue = Mask.MaskInt(lValue, aFirstParameter, aSecondParameter, aFunctionName).ToString();
                        }

                    }

                    break;
                case "varchar":
                case "nvarchar":
                case "varchar2":
                case "text":
                    {
                        string lValue = aValue;
                        if (aConditions != null)
                        {
                            foreach (var nCondition in aConditions)
                            {
                                if (!Mask.StrOperator(nCondition.Operator, lValue.ToLower(), nCondition.Value.ToLower()))
                                {
                                    lShouldMask = true;
                                    continue;
                                }
                                else
                                {
                                    lShouldMask = false; 
                                    break;
                                }
                            }
                            lMaskedValue = lShouldMask ? Mask.MaskString(lValue, aFirstParameter, aSecondParameter, aFunctionName).ToString() : lValue.ToString();
                        }
                        else
                        {
                            lMaskedValue = Mask.MaskString(lValue, aFirstParameter, aSecondParameter, aFunctionName);
                        }
                    }
                    break;
                case "date":
                case "datetime":
                case "datetime2":
                    {
                        DateTime lValue = DateTime.Parse(aValue);
                        if (aConditions != null)
                        {
                            foreach (var nCondition in aConditions)
                            {
                                if (!Mask.DateOperator(nCondition.Operator, lValue, DateTime.Parse(nCondition.Value)))
                                {
                                    lShouldMask = true;
                                    continue;  
                                }
                                else
                                {
                                    lShouldMask = false;
                                    break;
                                }
                            }
                            lMaskedValue = lShouldMask ? Mask.MaskDate(lValue, aFunctionName).ToString() : lValue.ToString();
                        }
                        else
                        {
                            lMaskedValue = Mask.MaskDate(lValue, aFunctionName).ToString();
                        }       
                    }
                    break;
                case "bit":
                    {
                        bool lValue = false;
                        if (Bool.IsBool(aValue))
                        {
                            lValue = Bool.ConvertBool(aValue);
                        }
                        else break;
                        if (aConditions != null)
                        {
                            foreach (var nCondition in aConditions)
                            {
                                bool lConditionValue = false;
                                if (Bool.IsBool(nCondition.Value))
                                {
                                    lConditionValue = Bool.ConvertBool(nCondition.Value);
                                }
                                else break;
                                if (!Mask.BoolOperator(nCondition.Operator, lValue, lConditionValue))
                                {
                                    lShouldMask = true;
                                    continue;
                                }
                                else
                                {
                                    lShouldMask = false;
                                    break;
                                }
                            }
                            lMaskedValue = lShouldMask ? Mask.MaskBool(lValue, aFunctionName).ToString() : lValue.ToString();
                        }
                        else
                        {
                            lMaskedValue = Mask.MaskBool(lValue, aFunctionName).ToString();
                        }
                    }
                    break;

            }
            return lMaskedValue;
        }
    }
}