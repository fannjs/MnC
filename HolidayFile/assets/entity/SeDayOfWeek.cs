using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HolidayFileModule.assets.entity
{
    /// <summary>
    /// For Kiosk usage
    /// </summary>
    public class SeDayOfWeek
    {
        public int DayOfWeekId { get; set; }
        public string DayOfWeekName { get; set; }

        public bool IsBusinessDay { get; set; }
    }
}
