using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Maestro.app_monitoring.DataObject
{
    //we don't need to store this on a list. As this query only returns 1 value.
    public class MachineDetails
    {
        public string ERROR
        {
            get;
            set;
        }
        public string STATUSCODE
        {
            get;
            set;
        }
        public string ALIVE
        {
            get;
            set;
        }
        public string SOP
        {
            get;
            set;
        }

        public string ERRORDESC
        {
            get;
            set;
        }

        public string DATE
        {
            get;
            set;
        }
        public string TIME
        {
            get;
            set;
        }
        public string MACHINEID
        {
            get;
            set;
        }

        public string CONTACTNUM
        {
            get;
            set;
        }
        public string CONTACTPERSON
        {
            get;
            set;
        }
        public string BRANCH
        {
            get;
            set;
        }
        public string ADDRESS1
        {
            get;
            set;
        }
        public string ADDRESS2
        {
            get;
            set;
        }
        public string VENDOR
        {
            get;
            set;
        }

        public string COUNTRY
        {
            get;
            set;
        }

        public string STATE
        {
            get;
            set;
        }

        public string DISTRICT
        {
            get;
            set;
        }

        public string CUSTOMER
        {
            get;
            set;
        }

        public string LAST_TRANS
        {
            get;
            set;
        }
        public bool LoadDataTable(DataTable dt)
        {
            //IT SHALL LOOP ONCE
            foreach (DataRow row in dt.Rows)
            {
                this.STATUSCODE = row[Intent.DB_ALIASE.STATUSCODE.ToString()].ToString();
                this.DATE = row[Intent.DB_ALIASE.DATE.ToString()].ToString();
                this.TIME = row[Intent.DB_ALIASE.TIME.ToString()].ToString();
                this.ALIVE = row[Intent.DB_ALIASE.M_ALIVE.ToString()].ToString();
                this.ERROR = TryFix(row[Intent.DB_ALIASE.ERROR.ToString()].ToString());
                this.ERRORDESC = row[Intent.DB_ALIASE.ERRORDESC.ToString()].ToString();

                this.MACHINEID = row[Intent.DB_ALIASE.MACHINEID.ToString()].ToString();
                this.BRANCH = row[Intent.DB_ALIASE.BRANCH.ToString()].ToString();
                this.VENDOR = row[Intent.DB_ALIASE.VENDOR.ToString()].ToString();

                this.COUNTRY = row[Intent.DB_ALIASE.COUNTRY.ToString()].ToString();
                this.DISTRICT = row[Intent.DB_ALIASE.DISTRICT.ToString()].ToString();
                this.STATE = row[Intent.DB_ALIASE.STATE.ToString()].ToString();
                this.CUSTOMER = row[Intent.DB_ALIASE.CUSTOMER.ToString()].ToString();


                this.ADDRESS1 = row[Intent.DB_ALIASE.ADDRESS1.ToString()].ToString();
                this.ADDRESS2 = row[Intent.DB_ALIASE.ADDRESS2.ToString()].ToString();
                this.CONTACTPERSON = row[Intent.DB_ALIASE.CONTACTPERSON.ToString()].ToString();
                this.CONTACTNUM = row[Intent.DB_ALIASE.CONTACTNUM.ToString()].ToString();

                this.SOP = row[Intent.DB_ALIASE.SOP.ToString()].ToString();
                break;

            }
            return true;
        }

        public string TryFix(string errType)
        {
            string fixStr = errType;
            if (errType.ToUpper().Equals("WARN"))
            {
                fixStr = "WARNING";
            }
            return fixStr;
        }

    }
}