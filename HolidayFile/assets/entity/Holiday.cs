using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HolidayFileModule.entity
{
    /// <summary>
    /// Modal: Holiday Container
    /// </summary>
    public class Holiday
    {

        public Header Head { get; set; }
        public List<Detail> Details { get; set; } 

        public class Header
        {
            // Header Section
            public int CalVersion { get; set; }
            public DateTime VersionDT { get; set; }
            public string DayOfWeekName { get; set; }
            public List<int> DayOfweekID { get; set; }
        }

        public class Detail
        {
            public DateTime HolidayDate { get; set; }
            public string HolidayName { get; set; }
            public bool IsBusinessDay { get; set; }
        }
    }



}
