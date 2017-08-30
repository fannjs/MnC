using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Maestro.app_monitoring.DataObject
{
    public class UrgencyQueueList
    {
        public List<UrgencyQueue> LIST
        {
            get;
            set;
        }

        public UrgencyQueueList()
        {
            LIST = new List<UrgencyQueue>();
        }

        public void AddUrgencyQueue(UrgencyQueue uq)
        {
            LIST.Add(uq);
        }

        public bool LoadDataTable(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                AddUrgencyQueue(new UrgencyQueue(
                    row[Intent.DB_ALIASE.MACHINEID.ToString()].ToString(),
                    row[Intent.DB_ALIASE.ERROR.ToString()].ToString(),
                    row[Intent.DB_ALIASE.MACHINETYPE.ToString()].ToString(),
                    row[Intent.DB_ALIASE.ERRORDESC.ToString()].ToString(),
                    row[Intent.DB_ALIASE.TIME.ToString()].ToString()
                ));
            }
            LIST.Sort(
                delegate(UrgencyQueue u1, UrgencyQueue u2)
                {

                    Intent.DB_DATA errTypeP1 = (Intent.DB_DATA)Enum.Parse(typeof(Intent.DB_DATA), u1.ERROR);
                    Intent.DB_DATA errTypeP2 = (Intent.DB_DATA)Enum.Parse(typeof(Intent.DB_DATA), u2.ERROR);

                    int compareErrorType = errTypeP1.CompareTo(errTypeP2);

                    return compareErrorType;
                }
            );



            return true;
        }
    }



    public class UrgencyQueue
    {
        public string MACHINEID
        {
            get;
            set;
        }
        public string ERROR
        {
            get;
            set;
        }
        public string MACHINETYPE
        {
            get;
            set;
        }
        public string ERRORDESC
        {
            get;
            set;
        }
        public string TIME
        {
            get;
            set;
        }

        public UrgencyQueue(string mID, string err, string mType, string errDesc, string time)
        {
            this.MACHINEID = mID;
            this.ERROR = err;
            this.MACHINETYPE = mType;
            this.ERRORDESC = errDesc;
            this.TIME = time;
        }
    }
}