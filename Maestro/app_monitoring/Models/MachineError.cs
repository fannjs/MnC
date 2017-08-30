using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Maestro.app_monitoring.Models
{
    public class MachineError
    {
        public string M_MACH_TYPE { get; set; }
        public string M_MACH_MODEL { get; set; }
        public string M_CODE { get; set; }
        public string M_ERROR_DESCRIPTION { get; set; }
        public string M_ERRORTYPE { get; set; }
        public string M_SENSOR_XY { get; set; }
    }
}