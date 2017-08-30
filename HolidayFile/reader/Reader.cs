using HolidayFileModule.entity;
using HolidayFileModule.utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HolidayFileModule.reader
{
    public class Reader
    {

        public string FDir { get; set; }


        public Reader(string fDir)
        {
            this.FDir = fDir;
        }

        /// <summary>
        /// ReadNBackUp
        /// </summary>
        /// <param name="holiday">Holiday type</param>
        /// <param name="readExt">extension with dot, ex: .dat</param>
        /// <param name="bkExt">bk file name, ex: .bak</param>
        /// <returns></returns>
        public Status ReadNBackUp(out Holiday holiday, string readExt, string bkFName)
        {
            holiday = null;
            Status s = Status.FAILURE;
            try {

                var files = Directory.GetFiles(this.FDir, "*" + readExt);

                foreach (string f in files)
                {
                    s = ReaderAction.ReadHoliday(out holiday, f, ReaderAction.ReadHolidayHeader, ReaderAction.ReadHolidayDetail);

                    if (s.Success)
                    {
                        string destFile = Path.Combine(this.FDir, bkFName);
                        if (File.Exists(destFile))
                        {
                            File.Delete(destFile);
                        }

                        File.Copy(f, destFile);//backup the file.
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                s.Ex = ex;
                s.Message = ex.Message;
                s.Success = false;
            }

            return s;
        }
    }
}
