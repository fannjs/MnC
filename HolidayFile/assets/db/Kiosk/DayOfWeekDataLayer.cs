using HolidayFileModule.assets.entity;
using HolidayFileModule.entity;
using RototypeIntl.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace HolidayFileModule.db.Kiosk
{
    public class DayOfWeekDataLayer: AbstractDataLayer
    {

        public DayOfWeekDataLayer(IDbConnector dbCon)
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
        
        
        public Status DeleteAll()
        {
            string errMsg = "";
            string q = string.Format("DELETE FROM {0}", tblName);
            bool writeSuccess = dbCon.Write(q);

            if (!writeSuccess)
            {
                errMsg = dbCon.Error.GetSimpleError();
            }

            return new Status() { Success = true, Message = errMsg, Ex = null };
        }

        public Status AddDayOfWeek(SeDayOfWeek dow)
        {
            string errMsg = "";
            string q = string.Format("INSERT INTO {0} (DAY_OF_WEEK_ID, DAY_OF_WEEK_NAME, IS_BUSINESSDAY) VALUES({1}, '{2}', {3}) ", tblName, dow.DayOfWeekId, dow.DayOfWeekName, dow.IsBusinessDay);
            bool writeSuccess = dbCon.Write(q);

            if (!writeSuccess)
            {
                errMsg = dbCon.Error.GetSimpleError();
            }

            return new Status() { Success = writeSuccess, Message = errMsg, Ex = null };
        }
        public Status GetWeekendList(out List<string> weekendList)
        {
            weekendList = new List<string>();

            DataTable dt;
            string errMsg = "";
            string q = string.Format("SELECT DAY_OF_WEEK_NAME FROM {0} WHERE IS_BUSINESSDAY = False", tblName);
            bool readSuccess = dbCon.Read(q, out dt);

            if (!readSuccess)
            {
                errMsg = dbCon.Error.GetSimpleError();
            }
            else
            {
                foreach (DataRow rec in dt.Rows)
                {
                    weekendList.Add(rec["DAY_OF_WEEK_NAME"].ToString().ToLower());
                }
                readSuccess = true;
            }
            return new Status() { Success = readSuccess, Message = errMsg, Ex = null };
        }
    }
}
