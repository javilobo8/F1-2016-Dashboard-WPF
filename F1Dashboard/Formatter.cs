using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Dashboard
{
    class Formatter
    {
        public static String Seconds(float seconds)
        {
            int plain_minutes = (int)(seconds / 60);
            string str_minutes = plain_minutes.ToString();
            if (plain_minutes < 10) str_minutes = '0' + str_minutes;

            float plain_seconds = seconds % 60;
            string str_seconds = plain_seconds.ToString("N3");
            if (plain_seconds < 10) str_seconds = '0' + str_seconds;

            return String.Format("{0}:{1}", str_minutes, str_seconds);
        }
    }
}
