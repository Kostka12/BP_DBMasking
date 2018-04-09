using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mask
{
    public class Float
    {
        private static readonly Random mRandom;

        static Float()
        {
            mRandom = new Random();
        }
        public static float Hash(float aValue)
        {
            if (aValue == 0)
            {
                aValue++;
            }
            var lValue = mRandom.Next(1, (int)aValue);
            return (aValue + lValue) / (float)2.0;
        }
    }
}
