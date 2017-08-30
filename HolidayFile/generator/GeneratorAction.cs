using HolidayFileModule.entity;
using HolidayFileModule.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HolidayFileModule
{

    /// <summary>
    /// Sets of Actions to be used in Generator Class. Users are welcomed to extend this class.
    /// </summary>
    public static class GeneratorAction
    {
        /// <summary>
        /// FormatHeader
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        public static string FormatHeader(Holiday.Header h)
        {
            string dowIDs = string.Join(",", h.DayOfweekID);
            string headLine = string.Format("{0}|{1}|{2}|{3}", h.CalVersion, h.VersionDT.ToString(Utility.FORMAT_ddMMyyyy), h.DayOfWeekName, dowIDs);

            return headLine;
        }
        /// <summary>
        /// FormatDetails
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        public static string FormatDetails(List<Holiday.Detail> dList)
        {
            string detail = "";
            StringBuilder sb = new StringBuilder();
            foreach (Holiday.Detail d in dList)
            {
                detail = string.Format("{0}|{1}|{2}",
                                    d.HolidayDate.ToString(Utility.FORMAT_dd_MM_yyyy),
                                    d.HolidayName,
                                    Utility.Bool2Int(d.IsBusinessDay)
                                    );

                sb.AppendLine(detail);
            }
            return sb.ToString();
        }
    }
}
