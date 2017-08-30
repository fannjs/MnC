using HolidayFileModule.db;
using HolidayFileModule.db.ConsoleServer;
using HolidayFileModule.entity;
using HolidayFileModule.facade;
using NUnit.Framework;
using RototypeIntl.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HolidayFileModule.UnitTests
{
    [TestFixture]
    class HolidayFacadeTest
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
        public void HolidayFacade__LoadHolidayHeader__AreEqual()
        {
            SpecialDayOfWeek sDow;
            g.LoadSpecialDayOfWeek(out sDow);
            SetupCalendar sc = new SetupCalendar() { CalVersion = 1, VersionFilePath = "mockedpath", VersionDT = DateTime.Now };

            Holiday.Header h = HolidayFacade.LoadHolidayHeader(sc, sDow);

            Assert.AreEqual(sc.CalVersion, h.CalVersion);
            Assert.AreEqual(sc.VersionDT, h.VersionDT);
            Assert.AreEqual(sDow.DayOfWeek, h.DayOfWeekName);
            Assert.AreEqual(sDow.DayOfWeekIdList, h.DayOfweekID);

        }

        [Test]
        public void HolidayFacade__LoadHolidayDetails__()
        {

        }

        [Test]
        public void HolidayFacade__LoadHoliday__()
        {

        }

    }
}
