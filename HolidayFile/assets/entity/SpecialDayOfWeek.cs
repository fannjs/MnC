using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HolidayFileModule.entity
{
    /// <summary>
    /// Used by Console Server
    /// </summary>
    public class SpecialDayOfWeek //as per design spec
    {
        public string DayOfWeek { get; set; }
        public List<int> DayOfWeekIdList { get; set; }


    }
}
