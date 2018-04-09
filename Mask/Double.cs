using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mask
{
    public class Double
    {
        private static readonly Random mRandom;

        static Double()
        {
            mRandom = new Random();
        }
        public static double Hash(double aValue)
        {
            if (aValue == 0)
            {
                aValue++;
            }
            var lValue = mRandom.Next(1, (int)aValue);
            return (aValue + lValue) / 2.0;
        }
    }
}
