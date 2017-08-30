using HolidayFileModule.agent;
using HolidayFileModule.db.Kiosk;
using HolidayFileModule.entity;
using NUnit.Framework;
using RototypeIntl.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HolidayFileModule.UnitTests
{
    [TestFixture]
    class HolidayAgentTest
    {
        DayOfWeekDataLayer dowdl;
        HolidayCalendarDataLayer hcdl;
        private IDbConnector tempCsdDB;
        private string cp = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\roy\Documents\Visual Studio 2012\Projects\MaestroSVN\HolidayFileModule.UnitTests\database\CSDKiosk.mdb" + ";Persist Security Info=False";
        private bool connected;
        
        
        [SetUp]
        public void Setup()
        {
            connected = SetupDBConn(cp);
            dowdl = new DayOfWeekDataLayer(tempCsdDB);
            hcdl = new HolidayCalendarDataLayer(tempCsdDB);
            
        }

        public bool SetupDBConn(string connParam)
        {
            ///warning, to test this piece of codes, if pc: 64bit, use nunit-86.exe, n change ur conf manager to x86 output (so that oledb dll will work)
            tempCsdDB = DatabaseFactory.CreateConnector(DatabaseFactory.DBTYPE.OLEDB, connParam);
            return tempCsdDB.OpenConnection();
        }


        [Test]
        public void InsertToDayOfWeek__Normal__ReturnTrue()
        {
            Status s =  HolidayAgent.InsertToDayOfWeek(new Holiday.Header() { DayOfweekID = new List<int>() { 1, 2, 3, 4, 5 }, DayOfWeekName = "Tuesday" }, dowdl);

            Assert.True(s.Success, s.Message);
        }


        [Test]
        public void GetNextBusinessDay__ReturnsTrue()
        {
            List<DateTime> hdList;
            Status s = hcdl.GetHolidayDates(out hdList);

            Assert.True(s.Success, s.Message);
            
            List<string> weekendList;
            s = dowdl.GetWeekendList(out weekendList);
            Assert.True(s.Success, s.Message);

            DateTime dt = HolidayAgent.GetNextBusinessDate(DateTime.Now, DateTime.Now.AddDays(2), weekendList, hdList);
           Assert.AreEqual(dt.ToString("dddd"),"Tuesday");
        }

    }
}
