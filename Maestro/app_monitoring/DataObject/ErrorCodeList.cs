using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Maestro.app_monitoring.DataObject
{
    public class ErrorCodeList
    {
        public List<ErrorCode> LIST
        {
            get;
            set;
        }

        public ErrorCodeList()
        {
            LIST = new List<ErrorCode>();
        }

        public bool LoadDataTable(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                AddErrorCode(
                    row[Intent.DB_ALIASE.STATUSCODE.ToString()].ToString(),
                    TryFix(row[Intent.DB_ALIASE.ERROR.ToString()].ToString()),
                    row[Intent.DB_ALIASE.ERRORDESC.ToString()].ToString(),
                    row[Intent.DB_ALIASE.SOP.ToString()].ToString(),
                    row[Intent.DB_ALIASE.CategoryID.ToString()].ToString(),
                    row[Intent.DB_ALIASE.ImageID.ToString()].ToString()
                    );
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

        public void AddErrorCode(string sCode, string err, string errDesc, string sop, string cID, string imgID)
        {
            LIST.Add(new ErrorCode(sCode, err, errDesc, sop, cID, imgID));
        }



    }

    public class ErrorCode
    {
        public String STATUSCODE
        {
            get;
            set;
        }
        public String ERROR
        {
            get;
            set;
        }
        public String ERRORDESC
        {
            get;
            set;
        }
        public String SOP { get; set; }

        public String CategoryID { get; set; }

        public String ImageID { get; set; }

        public ErrorCode(string sCode, string err, string errDesc, string sop, string cID, string imgID)
        {
            this.STATUSCODE = sCode;
            this.ERROR = err;
            this.ERRORDESC = errDesc;
            this.SOP = sop;
            this.CategoryID = cID;
            this.ImageID = imgID;
        }


    }
}