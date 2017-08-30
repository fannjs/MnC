using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HolidayFileModule.utility
{
    public static class Utility
    {

        public const string FORMAT_ddMMyyyy = "ddMMyyyy";
        public const string FORMAT_dd_MM_yyyy = "dd/MM/yyyy";


        public static bool Int2Bool(int val)
        {
            if (val < 0 && val > 1) throw new ArgumentOutOfRangeException("val is out of 0-1 range");

            return (val == 0) ? false : true;

        }
        public static int Bool2Int(bool val)
        {

            return (val)? 1 : 0;

        }


        public static void CheckOrCreateNewDirectory(string fDir)
        {
            if (!Directory.Exists(fDir))
            {
                Directory.CreateDirectory(fDir);
            }
        }

    }
}
