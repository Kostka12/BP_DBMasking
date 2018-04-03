using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mask
{
    public class Date
    {
        public static DateTime Hash(DateTime dT)
        {
            
            int d = dT.Day;
            int m = dT.Month;
            int y = dT.Year;
            int dx;
            int mx = 0;
            int yx = 0;
            if (d > m)
            {
                dx = (d%m) + m;
            }
            else
            {
                dx = (d*m*2)%(d + m);
            }
            while (y != 0)
            {
                mx += y % 10;
                y /= 10;
            }
            mx -= (d + m);
            y = dT.Year;
            if (mx < 0)
            {
                mx *= -1;
            }
            if (mx >= dx)
            {
                int tmp = dx;
                dx = mx;
                mx = tmp;
            }
            if (mx == 0)
            {
                mx++;
            }
            if (dx == 0)
            {
                dx++;
            }
            if (mx > 12 || dx > 28)
            {
                mx = mx%12;
                dx = dx%28; 
            }
            yx = y - m + d + mx - dx; 
                
            Console.WriteLine(dx +"."+mx+ "."+yx);
            DateTime mDT = new DateTime(yx,mx,dx);
            return mDT;
        }
    }
}
