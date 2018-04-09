using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mask
{
    public class String
    {
        private static readonly Random mRandom;
        public static Dictionary<string, string> Original_MaskedPairs;
        static String()
        {
            mRandom = new Random();
        }
        public static string ConcatenateEmail(string a, string b, string domain="@example.cz")
        {
            return a + "." + b + domain;
        }

        public static string RandStringPart(string str, int from, int to)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            int length = to - from;
            var rep = new string(Enumerable.Repeat(chars, length).Select(s => s[mRandom.Next(s.Length)]).ToArray());
            if (str.Length < from || str.Length-from<length)
            {
                return str;
            }
            else
            {
                string res = str.Remove(from, length);
                res = res.Insert(from, rep);
                return res;
            }    
        }

        public static string ReplaceStringPart(string str,int from ,char with = 'x')
        {
            int length = str.Length - from;
            
            if (str.Length < from || length<0)
            {
                return str;
            }
            else
            {
                string res = str.Remove(from, length);
                string rep = "";
                for (int i = 0; i < length; i++)
                {
                    rep += with;
                }
                res = res.Insert(from, rep);
                return res;
            }
            
        }

        public static string Substitution(string aValue)
        {
            if(Original_MaskedPairs != null)
            {

            }
            var lValue = Original_MaskedPairs.Where(aR => aR.Key == aValue).Select(aR => aR.Value).FirstOrDefault();
            if(lValue != null)
            {
                return lValue;
            }
            else
            {
                int lIndex = mRandom.Next(Mask.SubstitutionValues.Count);
                string lMaskedValue = Mask.SubstitutionValues[lIndex];
                Original_MaskedPairs.Add(aValue, lMaskedValue);
                Mask.SubstitutionValues.RemoveAt(lIndex);
                return lMaskedValue;
            }      
        }
        public static void NewOriginal_MaskedPairs()
        {
            Original_MaskedPairs = new Dictionary<string, string>();
        }
    }
}
