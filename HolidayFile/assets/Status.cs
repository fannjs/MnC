using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HolidayFileModule
{
    public class Status
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Exception Ex { get; set; }


        public static Status SUCCESS
        {
            get
            {
               return new Status() { Success = true, Message = "DEFAULT SUCCESS" }; //similar to getSuccess() func, less verbose
            }
        }

        public static Status FAILURE{
            get
            {
                return new Status() { Success = false, Message = "DEFAULT FAILURE" };
            }
        }
    }
}
