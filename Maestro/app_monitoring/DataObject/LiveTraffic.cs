using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Maestro.app_monitoring.DataObject
{
    public class LiveTraffic
    {
        public Dictionary<string, string> ERROR
        {
            get;
            set;
        }

        public LiveTraffic()
        {
            ERROR = new Dictionary<string, string>();
            ERROR.Add(Intent.DB_DATA.ERROR.ToString(), "0");
            ERROR.Add(Intent.DB_DATA.WARN.ToString(), "0");
            ERROR.Add(Intent.DB_DATA.ONLINE.ToString(), "0");
            ERROR.Add(Intent.DB_DATA.OFFLINE.ToString(), "0");
        }

        public void UpdateStatus(string err, string c)
        {
            if (ERROR.ContainsKey(err))
            {
                ERROR[err] = c;
            }
        }

        public bool LoadDataTable(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                UpdateStatus(
                    row[Intent.DB_ALIASE.ERROR.ToString()].ToString(),
                    row[Intent.DB_ALIASE.TOTALCOUNT.ToString()].ToString());
            }

            return true;
        }

    }
}