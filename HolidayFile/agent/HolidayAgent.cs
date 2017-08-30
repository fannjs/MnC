using HolidayFileModule.assets.entity;
using HolidayFileModule.db.Kiosk;
using HolidayFileModule.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HolidayFileModule.agent
{
    /// <summary>
    /// For Kiosk
    /// </summary>
    public static class HolidayAgent
    {
        private static Dictionary<string, Dictionary<int, SeDayOfWeek>> CacheDayOfWeekData =
        new Dictionary<string, Dictionary<int, SeDayOfWeek>>();

        static HolidayAgent (){


            Dictionary<int, SeDayOfWeek> cachedData = new Dictionary<int,SeDayOfWeek>();


            cachedData.Add(1, new SeDayOfWeek() { DayOfWeekId = 1, DayOfWeekName = "Monday", IsBusinessDay = false });
            cachedData.Add(2, new SeDayOfWeek() { DayOfWeekId = 2, DayOfWeekName = "Tuesday", IsBusinessDay = false });
            cachedData.Add(3, new SeDayOfWeek() { DayOfWeekId = 3, DayOfWeekName = "Wednesday", IsBusinessDay = false });
            cachedData.Add(4, new SeDayOfWeek() { DayOfWeekId = 4, DayOfWeekName = "Thursday", IsBusinessDay = false });
            cachedData.Add(5, new SeDayOfWeek() { DayOfWeekId = 5, DayOfWeekName = "Friday", IsBusinessDay = false });
            cachedData.Add(6, new SeDayOfWeek() { DayOfWeekId = 6, DayOfWeekName = "Saturday", IsBusinessDay = false });
            cachedData.Add(7, new SeDayOfWeek() { DayOfWeekId = 7, DayOfWeekName = "Sunday", IsBusinessDay = false });

            CacheDayOfWeekData.Add("Monday", cachedData);

            cachedData = new Dictionary<int, SeDayOfWeek>();
            cachedData.Add(1, new SeDayOfWeek() { DayOfWeekId = 1, DayOfWeekName = "Tuesday", IsBusinessDay = false });
            cachedData.Add(2, new SeDayOfWeek() { DayOfWeekId = 2, DayOfWeekName = "Wednesday", IsBusinessDay = false });
            cachedData.Add(3, new SeDayOfWeek() { DayOfWeekId = 3, DayOfWeekName = "Thursday", IsBusinessDay = false });
            cachedData.Add(4, new SeDayOfWeek() { DayOfWeekId = 4, DayOfWeekName = "Friday", IsBusinessDay = false });
            cachedData.Add(5, new SeDayOfWeek() { DayOfWeekId = 5, DayOfWeekName = "Saturday", IsBusinessDay = false });
            cachedData.Add(6, new SeDayOfWeek() { DayOfWeekId = 6, DayOfWeekName = "Sunday", IsBusinessDay = false });
            cachedData.Add(7, new SeDayOfWeek() { DayOfWeekId = 7, DayOfWeekName = "Monday", IsBusinessDay = false });



            CacheDayOfWeekData.Add("Tuesday", cachedData);

            cachedData = new Dictionary<int, SeDayOfWeek>();
            cachedData.Add(1, new SeDayOfWeek() { DayOfWeekId = 1, DayOfWeekName = "Wednesday", IsBusinessDay = false });
            cachedData.Add(2, new SeDayOfWeek() { DayOfWeekId = 2, DayOfWeekName = "Thursday", IsBusinessDay = false });
            cachedData.Add(3, new SeDayOfWeek() { DayOfWeekId = 3, DayOfWeekName = "Friday", IsBusinessDay = false });
            cachedData.Add(4, new SeDayOfWeek() { DayOfWeekId = 4, DayOfWeekName = "Saturday", IsBusinessDay = false });
            cachedData.Add(5, new SeDayOfWeek() { DayOfWeekId = 5, DayOfWeekName = "Sunday", IsBusinessDay = false });
            cachedData.Add(6, new SeDayOfWeek() { DayOfWeekId = 6, DayOfWeekName = "Monday", IsBusinessDay = false });
            cachedData.Add(7, new SeDayOfWeek() { DayOfWeekId = 7, DayOfWeekName = "Tuesday", IsBusinessDay = false });




            CacheDayOfWeekData.Add("Wednesday", cachedData);


            cachedData = new Dictionary<int, SeDayOfWeek>();
            cachedData.Add(1, new SeDayOfWeek() { DayOfWeekId = 1, DayOfWeekName = "Thursday", IsBusinessDay = false });
            cachedData.Add(2, new SeDayOfWeek() { DayOfWeekId = 2, DayOfWeekName = "Friday", IsBusinessDay = false });
            cachedData.Add(3, new SeDayOfWeek() { DayOfWeekId = 3, DayOfWeekName = "Saturday", IsBusinessDay = false });
            cachedData.Add(4, new SeDayOfWeek() { DayOfWeekId = 4, DayOfWeekName = "Sunday", IsBusinessDay = false });
            cachedData.Add(5, new SeDayOfWeek() { DayOfWeekId = 5, DayOfWeekName = "Monday", IsBusinessDay = false });
            cachedData.Add(6, new SeDayOfWeek() { DayOfWeekId = 6, DayOfWeekName = "Tuesday", IsBusinessDay = false });
            cachedData.Add(7, new SeDayOfWeek() { DayOfWeekId = 7, DayOfWeekName = "Wednesday", IsBusinessDay = false });


            CacheDayOfWeekData.Add("Thursday", cachedData);

            cachedData = new Dictionary<int, SeDayOfWeek>();
            cachedData.Add(1, new SeDayOfWeek() { DayOfWeekId = 1, DayOfWeekName = "Friday", IsBusinessDay = false });
            cachedData.Add(2, new SeDayOfWeek() { DayOfWeekId = 2, DayOfWeekName = "Saturday", IsBusinessDay = false });
            cachedData.Add(3, new SeDayOfWeek() { DayOfWeekId = 3, DayOfWeekName = "Sunday", IsBusinessDay = false });
            cachedData.Add(4, new SeDayOfWeek() { DayOfWeekId = 4, DayOfWeekName = "Monday", IsBusinessDay = false });
            cachedData.Add(5, new SeDayOfWeek() { DayOfWeekId = 5, DayOfWeekName = "Tuesday", IsBusinessDay = false });
            cachedData.Add(6, new SeDayOfWeek() { DayOfWeekId = 6, DayOfWeekName = "Wednesday", IsBusinessDay = false });
            cachedData.Add(7, new SeDayOfWeek() { DayOfWeekId = 7, DayOfWeekName = "Thursday", IsBusinessDay = false });




            CacheDayOfWeekData.Add("Friday", cachedData);

            cachedData = new Dictionary<int, SeDayOfWeek>();
            cachedData.Add(1, new SeDayOfWeek() { DayOfWeekId = 1, DayOfWeekName = "Saturday", IsBusinessDay = false });
            cachedData.Add(2, new SeDayOfWeek() { DayOfWeekId = 2, DayOfWeekName = "Sunday", IsBusinessDay = false });
            cachedData.Add(3, new SeDayOfWeek() { DayOfWeekId = 3, DayOfWeekName = "Monday", IsBusinessDay = false });
            cachedData.Add(4, new SeDayOfWeek() { DayOfWeekId = 4, DayOfWeekName = "Tuesday", IsBusinessDay = false });
            cachedData.Add(5, new SeDayOfWeek() { DayOfWeekId = 5, DayOfWeekName = "Wednesday", IsBusinessDay = false });
            cachedData.Add(6, new SeDayOfWeek() { DayOfWeekId = 6, DayOfWeekName = "Thursday", IsBusinessDay = false });
            cachedData.Add(7, new SeDayOfWeek() { DayOfWeekId = 7, DayOfWeekName = "Friday", IsBusinessDay = false });




            CacheDayOfWeekData.Add("Saturday", cachedData);


            cachedData = new Dictionary<int, SeDayOfWeek>();
            cachedData.Add(1, new SeDayOfWeek() { DayOfWeekId = 1, DayOfWeekName = "Sunday", IsBusinessDay = false });
            cachedData.Add(2, new SeDayOfWeek() { DayOfWeekId = 2, DayOfWeekName = "Monday", IsBusinessDay = false });
            cachedData.Add(3, new SeDayOfWeek() { DayOfWeekId = 3, DayOfWeekName = "Tuesday", IsBusinessDay = false });
            cachedData.Add(4, new SeDayOfWeek() { DayOfWeekId = 4, DayOfWeekName = "Wednesday", IsBusinessDay = false });
            cachedData.Add(5, new SeDayOfWeek() { DayOfWeekId = 5, DayOfWeekName = "Thursday", IsBusinessDay = false });
            cachedData.Add(6, new SeDayOfWeek() { DayOfWeekId = 6, DayOfWeekName = "Friday", IsBusinessDay = false });
            cachedData.Add(7, new SeDayOfWeek() { DayOfWeekId = 7, DayOfWeekName = "Saturday", IsBusinessDay = false });




            CacheDayOfWeekData.Add("Sunday", cachedData);







        }

        
        public static Status InsertToDayOfWeek(Holiday.Header h, DayOfWeekDataLayer dowdl)
        {
           // public string DayOfWeekName { get; set; }
            //public List<int>  DayOfweekID { get; set; }
             Dictionary<int, SeDayOfWeek> cachedData;
             SeDayOfWeek sdow;
             Status s = Status.SUCCESS;
             try
             {
                 if (CacheDayOfWeekData.TryGetValue(h.DayOfWeekName, out cachedData))
                 {
                     foreach (int bizID in h.DayOfweekID)
                     {
                         //update businessday true
                         if (cachedData.TryGetValue(bizID, out sdow))
                         {
                             sdow.IsBusinessDay = true;
                         }
                     }

                     //store in db
                     s = dowdl.DeleteAll(); //clear all before storing

                     if (!s.Success) throw new Exception("Failed to clear Day_Of_Week table");

                     foreach (var item in cachedData)
                     {
                         s = dowdl.AddDayOfWeek(item.Value);

                         if (!s.Success){ break;}
                     }
                 }
                 else
                 {
                     s.Success = false;
                     s.Message = "Something is wrong in retrieving cached day of week name";
                 }
             }
             catch (Exception ex)
             {
                 s.Ex = ex;
                 s.Success = false;
                 s.Message = ex.Message;
             }


            return s;
        }

        public static Status InsertToHolidayCalendar(List<Holiday.Detail> dList, HolidayCalendarDataLayer hcdl)
        {
            Status s = Status.SUCCESS;
            try
            {
                s = hcdl.DeleteAll();
                if (!s.Success) return s;
                List<HolidayCalendar> hcList = new List<HolidayCalendar>();

                foreach (Holiday.Detail d in dList)
                {
                    hcList.Add(new HolidayCalendar() { HolidayDate = d.HolidayDate, IsBusinessDay = d.IsBusinessDay, HolidayName = d.HolidayName });
                }

                s = hcdl.Add(hcList);

            }
            catch (Exception ex)
            {
                s.Success = false;
                s.Ex = ex;
                s.Message = ex.Message;
            }


            

            return s;

        }


        public static bool IsBusinessDay(DateTime date, List<string> weekendList, List<DateTime> holidayDateList)
        {
            bool isBusinessDay = !weekendList.Contains(date.ToString("dddd").ToLower()) && !holidayDateList.Contains(date);//faster
            return isBusinessDay;
        }

        public static DateTime GetNextBusinessDate(DateTime cutoffTm, DateTime today, List<string> weekendList, List<DateTime> holidayDateList)
        {
            DateTime nextDate = today;
            //if passed 4, add one then only check

            if (today > cutoffTm)
            {
                nextDate = today.AddDays(1);
            }


            while (!IsBusinessDay(nextDate, weekendList, holidayDateList)) {
                nextDate = nextDate.AddDays(1);
            }
            
            return nextDate;
        }
    }
}
