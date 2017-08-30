using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Maestro.app_monitoring.DataObject
{
    public class LocationStatusList
    {
        public string NEXT_LINK;

        public List<LocationStatus> LIST
        {
            get;
            set;
        }

        public LocationStatusList()
        {
            LIST = new List<LocationStatus>();
        }

        public bool LoadDataTable(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                AddLocationStatus(
                    row[Intent.DB_ALIASE.LOCATION.ToString()].ToString(),
                    row[Intent.DB_ALIASE.TOTALCOUNT.ToString()].ToString());
            }

            return true;
        }

        public void AddLocationStatus(string location, string c)
        {
            LIST.Add(new LocationStatus(location, c));
        }
    }

    public class LocationStatus
    {
        public string LOCATION
        {
            get;
            set;
        }
        public string COUNT
        {
            get;
            set;
        }
        public LocationStatus(string location, string count)
        {
            this.LOCATION = location;
            this.COUNT = count;
        }
    }
}