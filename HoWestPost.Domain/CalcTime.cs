using System;
using System.Collections.Generic;
using System.Text;

namespace HoWestPost.Domain
{
    public class CalcTime
    {
        public static double Mini (double mini)
        {
            return mini * 1;
        }
        public static double Standaard(double standaard)
        {
            return standaard * 1.2;
        }
        public static double Maxi(double maxi)
        {
            return maxi * 1.5;
        }
    }
}
