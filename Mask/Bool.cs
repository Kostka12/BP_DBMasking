using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mask
{
    public class Bool
    {
        private static readonly Random mRandom = new Random();
        public static bool RandomValue(bool aValue)
        {
            
            bool lRandom = mRandom.Next(2) == 0 ?  false :  true;
            return lRandom;
        }
        public static bool IsBool(string aValue)
        {
            if (aValue.ToLower() == "true" || aValue.ToLower() == "false")
            {
                return  true;
            }            
            else return false;
        }
        public static bool ConvertBool (string aValue)
        {
            bool lValue = false;
            if (aValue.ToLower() == "true")
                lValue = true;
            if (aValue.ToLower() == "false")
                lValue = false;

            return lValue;
        }
    }
}
