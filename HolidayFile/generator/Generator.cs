using HolidayFileModule.db;
using HolidayFileModule.db.ConsoleServer;
using HolidayFileModule.entity;
using HolidayFileModule.facade;
using HolidayFileModule.utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HolidayFileModule
{
    public class Generator
    {
        /*todo: a platform to
         * 0. Setup DB (temp)(completed)
         * 1. Deserialize data from DB (completed)
         * 2. Load values to Holiday Modal 
         * 3. Prepares CalendarFilename to be used as holiday file name 
         * 4. Prepares version,(completed)
         * 5. Get calendarfilename, get a path, once all done: store into table: save data.
         * */

        public delegate string HeaderFormatDelegate(Holiday.Header h);
        public delegate string DetailsFormatDelegate(List<Holiday.Detail> dList);

        public SetupCalendarDataLayer scDataLayer { get; private set; }
        public HolidayCalendarDataLayer hcDataLayer { get; private set; }
        public SpecialDayOfWeekDataLayer sdowDataLayer { get; private set; }

        public string FDir { get; set; }
        public Generator(HolidayCalendarDataLayer hcdl, SetupCalendarDataLayer scdl, SpecialDayOfWeekDataLayer sdowdl)
        {
            this.FDir = @"D:\";
            hcDataLayer = hcdl;
            scDataLayer = scdl;
            sdowDataLayer = sdowdl;
        }


        
        public Status LoadHolidayCalendars(out List<HolidayCalendar> hcList)
        {
            return HolidayCalendarFacade.GetAll(out hcList, hcDataLayer.GetAll);
        }

        public Status LoadSetupCalendar(out SetupCalendar sc)
        {   
            Status finalStatus = Status.FAILURE;
            sc = null;
            int nextVer;
            string generatedFName = "";
            Status sGetNextVersion = GetNextVersion(out nextVer);

            if (sGetNextVersion.Success)
            {
                generatedFName = GenerateHolidayFileName(DateTime.Now.Year, nextVer);
                sc = SetupCalendarFacade.LoadSetupCalendar(nextVer, DateTime.Now, Path.Combine(this.FDir, generatedFName));
                finalStatus = Status.SUCCESS;
            }

            return finalStatus;//HolidayCalendarFacade.GetAll(out hcList, hcDataLayer.GetAll);
        }
        
        public Status LoadSpecialDayOfWeek(out SpecialDayOfWeek sDow)
        {
            sDow = null;
            return SpecialDayOfWeekFacade.LoadSpecialDayOfWeek(sdowDataLayer.FindDayOfWeekName, sdowDataLayer.FindBusinessDayList, out sDow);
        }

        public string GenerateHolidayFileName(int year, int version)
        {
            string generatedFName = string.Format("CalendarFile{0}{1}.dat", year.ToString(), version);
            return generatedFName;
        }

        public Status GetNextVersion(out int nextVer)
        {
            nextVer = 0;
            int lastVer;
            Status s = SetupCalendarFacade.FindMaxCalVersion(out lastVer, scDataLayer.FindMaxCalVersion);

            if(s.Success)
            {
                nextVer = 1 + lastVer;
            }

            return s;
        }

        public Status InsertSetupInfo(SetupCalendar sc)
        {
            return SetupCalendarFacade.Add(sc, scDataLayer.Add);
        }

        /// <summary>
        /// Generate Holiday Calendar File, at the same time Insert setup info to DB upon completed.
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="holiday"></param>
        /// <param name="hFormat"></param>
        /// <param name="dFormat"></param>
        /// <returns></returns>
        public Status Generate(SetupCalendar sc, Holiday holiday, HeaderFormatDelegate hFormat, DetailsFormatDelegate dFormat)
        {
            Status s = Status.FAILURE;
            string fullFilePath = sc.VersionFilePath;
            try
            {
                Utility.CheckOrCreateNewDirectory(Path.GetDirectoryName(fullFilePath));

                FileStream fs = File.Open(fullFilePath, FileMode.Create);
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine(hFormat(holiday.Head));
                    writer.WriteLine(dFormat(holiday.Details));
                }

                s = InsertSetupInfo(sc);
            }
            catch (Exception ex)
            {
                s.Ex = ex;
                s.Message = ex.Message;
                s.Success = false;
            }

            return s;
        }




        //Loader -------------------------------------------------------------------------------------------------

        public delegate Status DelegateLoadSpecialDayOfWeek(out SpecialDayOfWeek sdow);
        public delegate Status DelegateLoadSetupCalendar(out SetupCalendar sc);
        public delegate Status DelegateLoadHolidayCalendars(out List<HolidayCalendar> hcList);

        public Status LoadHolidayHeader(out Holiday.Header h, DelegateLoadSetupCalendar dlsc, DelegateLoadSpecialDayOfWeek dlsdow)
        {
            h = null;
            SetupCalendar sc;
            Status s = dlsc(out sc);

            if(s.Success)
            {
                SpecialDayOfWeek sdow;
                s = dlsdow(out sdow);
                if (s.Success)
                {
                    h = HolidayFacade.LoadHolidayHeader(sc, sdow);
                }
            }

            return s;
        }

        public Status LoadHolidayDetails(out List<Holiday.Detail> dList, DelegateLoadHolidayCalendars dlhc)
        {
            dList = null;
            List<HolidayCalendar> hcList;

            Status s = dlhc(out hcList);
            
            if (s.Success)
            {
                dList = HolidayFacade.LoadHolidayDetails(hcList);
            }

            return s;
        }

        public Status LoadHoliday(out Holiday holiday)
        {
            holiday = null;
            Holiday.Header h;
            Status s = this.LoadHolidayHeader(out h, this.LoadSetupCalendar, this.LoadSpecialDayOfWeek);

            if (s.Success)
            {
                List<Holiday.Detail> dList;
                s = this.LoadHolidayDetails(out dList, this.LoadHolidayCalendars);

                if (s.Success)
                {
                    holiday = HolidayFacade.LoadHoliday(h, dList);
                }
            }

            return s;
        }


    }
}
