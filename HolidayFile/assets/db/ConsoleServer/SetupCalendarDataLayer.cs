using HolidayFileModule.entity;
using RototypeIntl.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace HolidayFileModule.db.ConsoleServer
{
    /// <summary>
    /// SetupCalendar Class that interface-ing with Database
    /// </summary>
    public class SetupCalendarDataLayer : AbstractDataLayer
    {
        public SetupCalendarDataLayer(IDbConnector dbCon)
            : base("SETUP_CALENDAR", dbCon)
        { }


        /// <summary>
        /// Find maximum current version
        /// </summary>
        /// <param name="maxCalVersion"></param>
        /// <returns></returns>
        public Status FindMaxCalVersion(out int maxCalVersion)
        {
            DataTable dt;
            maxCalVersion = -1;
            string errMsg = "";
            string q = string.Format("SELECT MAX(CAL_VERSION) AS MAX_CAL_VERSION FROM {0}", tblName);
            bool readSuccess = dbCon.Read(q, out dt);

            if (!readSuccess)
            {
                errMsg = dbCon.Error.GetSimpleError();
            }
            else
            {
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["MAX_CAL_VERSION"].ToString() != "") //empty record found
                    {
                        maxCalVersion = int.Parse(dt.Rows[0]["MAX_CAL_VERSION"].ToString());
                    }
                }
            }

            return new Status() { Success = readSuccess, Message = errMsg, Ex = null };
        }


        /// <summary>
        /// add to DB
        /// </summary>
        /// <param name="maxCalVersion"></param>
        /// <returns></returns>
        public Status Add(SetupCalendar sc)
        {
            string errMsg = "";
            string q = string.Format("INSERT INTO {0} (CAL_VERSION, CAL_VERSION_FILEPATH, VERSION_DT) VALUES({1}, '{2}', '{3}') ", tblName, sc.CalVersion, sc.VersionFilePath, sc.VersionDT.ToString((sc.VersionDTFormat == "") ? "dd/MM/yyyy/" : sc.VersionDTFormat));
            bool writeSuccess = dbCon.Write(q);

            if (!writeSuccess)
            {
                errMsg = dbCon.Error.GetSimpleError();
            }

            return new Status() { Success = writeSuccess, Message = errMsg, Ex = null };
        }


    }
}
