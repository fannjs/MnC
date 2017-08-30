using HolidayFileModule.entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace HolidayFileModule.facade
{
    public static class HolidayCalendarFacade
    {
        public delegate Status GetAllDelegate (out List<HolidayCalendar> list);

        /// <summary>
        /// Deserialize a list of holiday calendar entries.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Status GetAll(out List<HolidayCalendar> list, GetAllDelegate delg)
        {
            Status status = new Status();
            list = null;

            try
            {
                status = delg(out list);
            }
            catch (Exception ex)
            {
                status.Ex = ex;
                status.Success = false;
                status.Message = ex.ToString();
            }
            return status;
        }

    }
}
