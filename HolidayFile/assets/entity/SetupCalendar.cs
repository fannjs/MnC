using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HolidayFileModule.entity
{
    public class SetupCalendar
    {
        public int CalVersion { get; set; }
        public string VersionFilePath { get; set; }
        public DateTime VersionDT { get; set; }
        public string VersionDTFormat { get; set; }
    }

}
