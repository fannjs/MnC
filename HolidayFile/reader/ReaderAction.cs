using HolidayFileModule.entity;
using HolidayFileModule.utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace HolidayFileModule.reader
{
    public static class ReaderAction
    {
        public delegate Holiday.Header DelegateReadHolidayHeader(string h);
        public delegate Holiday.Detail DelegateReadHolidayDetail(string d);

        public static Holiday.Header ReadHolidayHeader(string h)
        {
            string[] hDatas = h.Split('|');
            Holiday.Header head = new Holiday.Header();
            head.DayOfweekID = new List<int>();

            if (hDatas.Length != 4) throw new Exception("Read File Operation: Invalid Holiday Header Data Length: " + h);

            head.CalVersion = int.Parse(hDatas[0]);
            head.VersionDT = DateTime.ParseExact(hDatas[1], Utility.FORMAT_ddMMyyyy, CultureInfo.InvariantCulture);
            head.DayOfWeekName = hDatas[2];
           
            //spliting comma
            string[] dowIds = hDatas[3].Split(',');
            if (dowIds.Length < 1) throw new Exception("Read File Operation: Invalid Holiday Header Data: DayOfWeekIDs Length:" + hDatas[3]);

            foreach (string dowId in dowIds)
            {
                head.DayOfweekID.Add(int.Parse(dowId));
            }

            return head;
        }

        public static Holiday.Detail ReadHolidayDetail(string d)
        {
            string[] dDatas = d.Split('|');
            Holiday.Detail det = new Holiday.Detail();

            if (dDatas.Length != 3) throw new Exception("Read File Operation: Invalid Holiday Detail Data:");

            det.HolidayDate = DateTime.ParseExact(dDatas[0], Utility.FORMAT_dd_MM_yyyy, CultureInfo.InvariantCulture);
            det.HolidayName = dDatas[1];
            det.IsBusinessDay = Utility.Int2Bool(int.Parse(dDatas[2]));

            return det;
        }

        /// <summary>
        /// Accepting delegates to read both head and detail
        /// </summary>
        /// <param name="dlgHolHead"></param>
        /// <param name="dlgHolDet"></param>
        public static Status ReadHoliday(out Holiday holiday, string fullFilePath, DelegateReadHolidayHeader dlgHolHead, DelegateReadHolidayDetail dlgHolDet)
        {
            holiday = new Holiday();
            holiday.Details = new List<Holiday.Detail>();

            Status s = Status.FAILURE;
            try
            {
                if (!File.Exists(fullFilePath)) throw new FileNotFoundException("Invalid file path:" + fullFilePath);

                using (StreamReader reader = new StreamReader(fullFilePath))
                {
                    string headLine, bodyLine;
                    headLine = reader.ReadLine();
                    holiday.Head = dlgHolHead(headLine);
                    
                     
                    while ((bodyLine = reader.ReadLine()) != null)
                    {
                        bodyLine = bodyLine.Trim();
                        if (bodyLine.Length < 1)
                        {
                            continue;
                        }
                        holiday.Details.Add(dlgHolDet(bodyLine));
                    }

                    if (holiday.Head != null && holiday.Details != null)
                    {
                        s.Success = true;
                    }
                }

            }
            catch (Exception ex)
            {
                s.Success = false;
                s.Message = ex.Message;
                s.Ex = ex;
            }

            return s;

        }

    }
}
