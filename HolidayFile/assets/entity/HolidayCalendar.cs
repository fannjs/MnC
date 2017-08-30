using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HolidayFileModule.entity
{
    public class HolidayCalendar
    {
        public DateTime HolidayDate { get; set; } 
        public string HolidayName { get; set; }
        public bool IsBusinessDay { get; set; }   //from day_of_week -> is_businessday
    }
}
