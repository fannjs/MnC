using HolidayFileModule.entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace HolidayFileModule.facade
{
    /// <summary>
    /// Facade guides user how to deserialize from source to data.
    /// </summary>
    public static class SetupCalendarFacade
    {
        public delegate Status FindMaxCalVersionDelegate(out int maxCalVersion);
        public delegate Status AddDelegate(SetupCalendar sc);


        /// <summary>
        /// API to find max current version
        /// </summary>
        /// <param name="maxCalVersion"></param>
        /// <param name="delg"></param>
        /// <returns></returns>
        public static Status FindMaxCalVersion(out int maxCalVersion, FindMaxCalVersionDelegate delg)
        {
            Status status = new Status();

            maxCalVersion = 0;
            try
            {
                status = delg(out maxCalVersion);
            }
            catch (Exception ex)
            {
                status.Ex = ex;
                status.Success = false;
                status.Message = ex.ToString();
            }
            return status;
        }

        /// <summary>
        /// API to serialize data
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="delg"></param>
        /// <returns></returns>
        public static Status Add(SetupCalendar sc, AddDelegate delg)
        {
            Status status = new Status();

            try
            {
                status = delg(sc);
            }
            catch (Exception ex)
            {
                status.Ex = ex;
                status.Success = false;
                status.Message = ex.ToString();
            }
            return status;
        }

        public static SetupCalendar LoadSetupCalendar(int version, DateTime dt, string fullFilePath)
        {
            SetupCalendar sc = new SetupCalendar()
            {
                CalVersion = version,
                VersionDT = dt,
                VersionFilePath = fullFilePath
            }; 
            return sc;//HolidayCalendarFacade.GetAll(out hcList, hcDataLayer.GetAll);
        }
    }
}
