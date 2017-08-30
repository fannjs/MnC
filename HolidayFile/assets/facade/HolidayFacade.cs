using HolidayFileModule.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HolidayFileModule.facade
{
    /// <summary>
    /// Facade guides user how to deserialize from source to data.
    /// </summary>
    public class HolidayFacade
    {
        public static Holiday.Header LoadHolidayHeader(SetupCalendar sc, SpecialDayOfWeek sdow)
        {
            Holiday.Header header = new Holiday.Header()
            {
                CalVersion = sc.CalVersion,
                VersionDT = sc.VersionDT,
                DayOfWeekName = sdow.DayOfWeek,
                DayOfweekID = sdow.DayOfWeekIdList
            };
            return header;
        }

        public static List<Holiday.Detail> LoadHolidayDetails(List<HolidayCalendar> hList)
        {
            List<Holiday.Detail> dList = new List<Holiday.Detail>();
            Holiday.Detail d = null;
            foreach (HolidayCalendar h in hList)
            {
                d = new Holiday.Detail()
                {
                    HolidayDate = h.HolidayDate,
                    HolidayName = h.HolidayName,
                    IsBusinessDay = h.IsBusinessDay,
                };
                dList.Add(d);
            
            }
            return dList;
        }

        public static Holiday LoadHoliday(Holiday.Header h, List<Holiday.Detail> dList)
        {
            return new Holiday()
            {
                Head = h,
                Details = dList
            };
        }
    }
}
