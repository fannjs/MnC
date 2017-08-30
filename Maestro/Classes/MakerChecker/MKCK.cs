using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Maestro.Classes.MakerChecker
{
    public class MKCK
    {
        public string ItemID { get; set; }
        public string TaskID { get; set; }
        public string TaskName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? RepliedDate { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        public string Action { get; set; }
        public string ActionDesc { get; set; }
        public string SQL { get; set; }
        public string MakerID { get; set; }
        public string MakerName { get; set; }
        public string MCStatus { get; set; }
        public string StatusDesc { get; set; }
        public string CheckerID { get; set; }
        public string CheckerName { get; set; }
        public string Remark { get; set; }
    }
}