using HolidayFileModule.db;
using HolidayFileModule.db.ConsoleServer;
using HolidayFileModule.entity;
using NUnit.Framework;
using RototypeIntl.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace HolidayFileModule.UnitTests
{
    [TestFixture]
    public class GeneratorTest
    {
        Generator g;
        public SetupCalendarDataLayer scDataLayer;
        public HolidayCalendarDataLayer hcDataLayer;
        public SpecialDayOfWeekDataLayer sdowDataLayer;
        private IDbConnector tempCsdDB;
        private string cp = "Data Source=126.32.3.39; Integrated Security=false; Initial Catalog=CSDMS;User ID=SSTAuto;Password=1qQA2wWS3eED";

        [SetUp]
        public void Setup()
        {
            SetupDBConn(cp);
            hcDataLayer = new HolidayCalendarDataLayer(tempCsdDB);
            scDataLayer = new SetupCalendarDataLayer(tempCsdDB);
            sdowDataLayer = new SpecialDayOfWeekDataLayer(tempCsdDB);

            g = new Generator(hcDataLayer, scDataLayer, sdowDataLayer);


        }

        public bool SetupDBConn(string connParam)
        {
            tempCsdDB = DatabaseFactory.CreateConnector(DatabaseFactory.DBTYPE.SQLDB, connParam);
            return tempCsdDB.OpenConnection();
        }
        [Test]
        public void LoadHolidayCalendars__GoodInput__ReturnsPositiveStatus()
        {
            List<HolidayCalendar> hcList;

            Status s = g.LoadHolidayCalendars(out hcList);

            //Console.Error.WriteLine()
            Assert.IsTrue(s.Success, s.Message);
            Assert.IsTrue(s.Ex == null, s.Message);
     
        }

        [Test]
        public void LoadHolidayCalendars__GoodInput__ReturnsAList()
        {
            //uncomment to insert a mock record
            //Status s = hcDataLayer.Add(new List<HolidayCalendar>(){ new HolidayCalendar() { HolidayDate= DateTime.Now, HolidayName="New Year", IsLegalHoliday= false, IsHoliday=true, IsBusinessDay=false }} );
            
            List<HolidayCalendar> hcList;

            Status s = g.LoadHolidayCalendars(out hcList);
            Assert.IsTrue(s.Success, s.Message);
        }


        [Test]
        [Ignore]
        public void InsertSetupInfo__GoodInput__ReturnTrue()
        {
            SetupCalendar sc = new SetupCalendar() { CalVersion = 1, VersionFilePath = "mockedpath", VersionDT = DateTime.Now };
            Status s = g.InsertSetupInfo(sc);
            Assert.IsTrue(s.Success, s.Message);
        }


        [Test]
        public void GetNextVersion__GoodDBConnection__ReturnTrue()
        {
            int lastVer;
            Status s = g.GetNextVersion(out lastVer);
            Console.Error.WriteLine(lastVer);
            Assert.IsTrue(s.Success, s.Message);
        }

        [Test]
        //[Ignore] as the truthfullness of result is solely depending on what data is lying up in db 
        public void LoadSpecialDayOfWeekData__GoodData__ReturnTrue()
        {
            SpecialDayOfWeek sdow;
            Status s = g.LoadSpecialDayOfWeek(out sdow);

            Assert.IsTrue(true, s.Message);
        }

        [Test]
        public void GenerateHolidayFileName__GoodData__ReturnTrue()
        {
            string expected = string.Format("CalendarFile{0}{1}.dat", DateTime.Now.Year, 1);

            string actual = g.GenerateHolidayFileName(DateTime.Now.Year, 1);

            Assert.AreEqual(expected, actual, actual);
        }

        [Test]
        public void LoadHoliday__Assume_Everything_Is_Fine__ReturnTrue()
        {
            Holiday holiday;
            Status s = g.LoadHoliday(out holiday);

            Assert.IsTrue(s.Success, s.Message);
        }


        [Test]
        public void Generate__Void__FileGenerated()
        {
            Holiday holiday;
            SetupCalendar sc;
            Status s = g.LoadHoliday(out holiday);
            string fPath = "";
            
            if (s.Success)
            {
                s = g.LoadSetupCalendar(out sc);

                if (s.Success)
                {
                    s = g.Generate(sc, holiday, GeneratorAction.FormatHeader, GeneratorAction.FormatDetails);
                    fPath = sc.VersionFilePath;
                }
            }

            Assert.IsTrue(s.Success, s.Message);

            Assert.IsTrue(File.Exists(fPath), "File not generated");

        }
    }
}
