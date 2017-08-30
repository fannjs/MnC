using HolidayFileModule.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HolidayFileModule.facade
{
    /// <summary>
    /// Facade guides user how to deserialize from source to data.
    /// </summary>
    public static class SpecialDayOfWeekFacade
    {
        public delegate Status FindDayOfWeekNameDelegate(out string dowName);
        public delegate Status FindBusinessDayListDelegate(out List<int> bizzList);
        public static Status LoadSpecialDayOfWeek(FindDayOfWeekNameDelegate dayOfWeekNameDelg, FindBusinessDayListDelegate businessDayListDelg, out SpecialDayOfWeek sDow)
        {
            List<int> bizzList;
            string firstDayOfWeek;
            sDow = null;
            Status finalStatus = Status.FAILURE;

            try
            {
                Status sBizzList = businessDayListDelg(out bizzList);

                if (sBizzList.Success)
                {
                    Status sDowName = dayOfWeekNameDelg(out firstDayOfWeek);
                    if (sDowName.Success)
                    {
                        sDow = new SpecialDayOfWeek()
                        {
                            DayOfWeek = firstDayOfWeek,
                            DayOfWeekIdList = bizzList
                        };

                        finalStatus = Status.SUCCESS;
                    }
                    else
                    {
                        finalStatus = sDowName;//state where u failed
                    }
                }
                else
                {
                    finalStatus = sBizzList;//state where u failed
                }
            }
            catch (Exception ex)
            {
                finalStatus.Success = false;
                finalStatus.Message = ex.Message;
                finalStatus.Ex = ex;
            }


            return finalStatus;
        }
    }
}
