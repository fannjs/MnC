using HolidayFileModule.entity;
using HolidayFileModule.utility;
using RototypeIntl.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace HolidayFileModule.db.ConsoleServer
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
                    q = string.Format("INSERT INTO  {0} (HOLIDAY_DATE, HOLIDAY_NAME, HOLIDAY_NOTE, DAY_OF_WEEK_ID, HOLIDAY_YEAR, DAY_OF_MONTH_ID, MONTH_OF_YEAR_ID, CREATED_DATE) VALUES('{1}', '{2}', '{3}', '{4}', '{5}', {6}, {7}, {8}, {9}, '{10}') ", tblName, 
                                      hc.HolidayDate.ToShortDateString(),
                                      hc.HolidayName,
                                      "",
                                      3,
                                      2014,
                                      1,
                                      1,
                                      DateTime.Now.ToShortDateString()
                                     );

                    writeSuccess = dbCon.Write(q);
                    if (!writeSuccess)
                    {
                        errMsg = dbCon.Error.GetSimpleError();
                        break;
                    }
            }
            return new Status() { Success = writeSuccess, Message = errMsg, Ex = null };
        }

    }
}
