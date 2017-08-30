using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Maestro.app_monitoring.content
{
    public class MaestroDateTime
    {

        public List<int> getDaysInYearMonth(int year, int month)
        {
            int days = DateTime.DaysInMonth(year, month);

            List<int> listofInt = new List<int>();
            for (int i = 1; i <= days; i++)
            {

                listofInt.Add(i);

            }
            return listofInt;

        }

        public List<int> getMonthsInYear()
        {
            List<int> listofInt = new List<int>();

            for (int i = 1; i <= 12; i++)
            {

                listofInt.Add(i);

            }
            return listofInt;
        }

        public List<int> getYearsFromNow()
        {

            List<int> listofInt = new List<int>();

            for (int i = 1980; i <= 2050; i++)
            {

                listofInt.Add(i);

            }
            return listofInt;
        }


        public List<int> getHours()
        {
            List<int> listofInt = new List<int>();

            for (int i = 0; i <= 23; i++)
            {

                listofInt.Add(i);

            }
            return listofInt;

        }

        public List<int> getMinutes()
        {
            List<int> listofInt = new List<int>();

            for (int i = 0; i <= 59; i++)
            {

                listofInt.Add(i);

            }
            return listofInt;

        }


        public List<int> getSeconds()
        {
            List<int> listofInt = new List<int>();

            for (int i = 0; i <= 59; i++)
            {

                listofInt.Add(i);

            }
            return listofInt;

        }
    }
}