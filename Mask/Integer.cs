using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mask
{
    public class Integer
    {
        private static readonly Random Rnd;

        static Integer()
        {
            Rnd = new Random();
        }
        public static int RandomValue(int x)
        {
            return 1;
        }

        public static int RangedValue(int from, int to)
        {
            var value = Rnd.Next(from,to);
            return value;
        }

        public static int Hash(int x)
        {
            if (x <= 0)
            {
                x *= -1;
                x++;
            }
            var value = Rnd.Next(1, x);
            value = (x + value)/2;
            return value;
            
        }

        public static int Shuffle(int index,int value1, int value2)
        {

            return 0;
        }
        public static int Offset(int x, int bottom, int top)
        { 
            
            var value = Rnd.Next(x - bottom, x + top);
            return value;
        }
    }
}
