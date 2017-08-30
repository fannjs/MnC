using HolidayFileModule.entity;
using RototypeIntl.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace HolidayFileModule.data.db
{
    public class SpecialDayOfWeekDataLayer: AbstractDataLayer
    {

        public SpecialDayOfWeekDataLayer(IDbConnector dbCon)
            : base("DAY_OF_WEEK", dbCon)
        { }

        public Status FindDayOfWeekName(out string dowName)
        {
            DataTable dt;
            string errMsg = "";
            dowName = ""; //empty default dowName
            string q = string.Format("SELECT DAY_OF_WEEK_NAME FROM {0} WHERE DAY_OF_WEEK_ID = 1", tblName);
            bool readSuccess = dbCon.Read(q, out dt);

            if (!readSuccess)
            {
                errMsg = dbCon.Error.GetSimpleError();
            }
            else
            {
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["DAY_OF_WEEK_NAME"].ToString() != "") //empty record found
                    {
                        dowName = dt.Rows[0]["DAY_OF_WEEK_NAME"].ToString();
                    }
                    else
                    {
                        //empty record
                        readSuccess = false;
                        errMsg = "empty record found";
                    }
                }
            }

            return new Status() { Success = readSuccess, Message = errMsg, Ex = null };
        }

        public Status FindBusinessDayList(out List<int> bizzList)
        {
            DataTable dt;
            string errMsg = "";
            bizzList = new List<int>() ; //empty default bussiness day list
            string q = string.Format("SELECT DAY_OF_WEEK_ID FROM {0} WHERE IS_BUSINESSDAY = 1", tblName);//*in ms-sql, TRUE/FALSE is reserved word  & therefore cannot be used as query.
            bool readSuccess = dbCon.Read(q, out dt);

            if (!readSuccess)
            {
                errMsg = dbCon.Error.GetSimpleError();
            }
            else
            {
                int dayofweekID = -999;
                foreach (DataRow rec in dt.Rows)
                {
                    dayofweekID = int.Parse(rec["DAY_OF_WEEK_ID"].ToString());
                    bizzList.Add(dayofweekID);
                }
            }

            return new Status() { Success = readSuccess, Message = errMsg, Ex = null };
        }



    }
}
