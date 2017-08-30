using HolidayFileModule.entity;
using HolidayFileModule.utility;
using RototypeIntl.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace HolidayFileModule.db.Kiosk
{
    public class HolidayCalendarDataLayer: AbstractDataLayer
    {
        public HolidayCalendarDataLayer(IDbConnector dbCon)
            :base("HOLIDAY_CALENDAR", dbCon)
        { }

        public Status GetAll(out List<HolidayCalendar> holidayList)
        {
            DataTable dt;
            string errMsg = "";
            string refTblName = "DAY_OF_WEEK";
            string q = string.Format("SELECT HC.HOLIDAY_DATE, HC.HOLIDAY_NAME, DOW.IS_BUSINESSDAY " +
                                     "FROM {0} AS HC, {1} AS DOW WHERE HC.DAY_OF_WEEK_ID = DOW.DAY_OF_WEEK_ID"
                                     ,tblName, refTblName);
             
            bool readSuccess = dbCon.Read(q, out dt);
            Status s = Status.FAILURE;

            holidayList = new List<HolidayCalendar>();

            if (!readSuccess)
            {
                errMsg = dbCon.Error.GetSimpleError();
            }
            else
            {   
                //int maxCalVersion
                //read & store as a list
                foreach (DataRow rec in dt.Rows)
                {
                    DateTime hd = DateTime.Parse(rec["HOLIDAY_DATE"].ToString());
                    string holidayName = rec["HOLIDAY_NAME"].ToString();
                    bool isBusinessDay = bool.Parse(rec["IS_BUSINESSDAY"].ToString());

                    holidayList.Add(new HolidayCalendar() { HolidayDate = hd, HolidayName = holidayName, IsBusinessDay = isBusinessDay });
                }
            }

            s.Success = readSuccess;

            return s;
        }
        public Status Add(List<HolidayCalendar> holidayList)
        {
            string errMsg = "";
            string q = "";
            bool writeSuccess = false;

            foreach(HolidayCalendar hc in holidayList)
            {
                    q = string.Format("INSERT INTO  {0} (HOLIDAY_DATE, HOLIDAY_NAME, IS_BUSINESSDAY) VALUES('{1}', '{2}', {3}) ", 
                                    tblName,
                                    hc.HolidayDate.ToString(Utility.FORMAT_dd_MM_yyyy),
                                    hc.HolidayName.Replace("'", "''"),
                                    hc.IsBusinessDay.ToString());

                    writeSuccess = dbCon.Write(q);
                    if (!writeSuccess)
                    {
                        errMsg = dbCon.Error.GetSimpleError();
                        break;
                    }
            }
            return new Status() { Success = writeSuccess, Message = errMsg, Ex = null };
        }
        /// <summary>
        /// Delete all records.
        /// </summary>
        /// <returns>Always return true.</returns>
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


        public Status GetHolidayDates(out List<DateTime> holidayDates)
        {
            holidayDates = new List<DateTime>();

            DataTable dt;
            string errMsg = "";
            string q = string.Format("SELECT HOLIDAY_DATE FROM {0}", tblName);
            bool readSuccess = dbCon.Read(q, out dt);

            if (!readSuccess)
            {
                errMsg = dbCon.Error.GetSimpleError();
            }
            else
            {
                DateTime date;
                foreach (DataRow rec in dt.Rows)
                {
                    date = DateTime.Parse(rec["HOLIDAY_DATE"].ToString());
                    holidayDates.Add(date);
                }
                readSuccess = true;
            }
            return new Status() { Success = readSuccess, Message = errMsg, Ex = null };
        }



    }
}
