using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Maestro.Classes.Prefix
{
    public class MCStatus
    {
        public static readonly int PENDING = 1;
        public static readonly int APPROVED = 2;
        public static readonly int REJECTED = 3;
        public static readonly int NOTED = 4;
        public static readonly int FAILED = 5;
        public static readonly int DELETED = 6;
    }
    public class MCAction
    {
        public static readonly string ADD = "A";
        public static readonly string MODIFY = "M";
        public static readonly string DELETE = "D";
    }
    public class MCDelimiter
    {
        public static readonly string SQL = "||||";
    }
}