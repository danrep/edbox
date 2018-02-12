using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdBox.Core
{
    public static class Computation
    {
        public static string GetGrade(decimal score)
        {
            if (score >= 0 && score < 40)
                return "F";
            if (score >= 40 && score < 46)
                return "E";
            if (score >= 46 && score < 50)
                return "D";
            if (score >= 50 && score < 60)
                return "C";
            if (score >= 60 && score < 70)
                return "B";
            if (score >= 70 && score < 101)
                return "A";

            return "NA";
        }

        public static string AddOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }
    }
}
