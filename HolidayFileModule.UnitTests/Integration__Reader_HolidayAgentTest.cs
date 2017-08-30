using HolidayFileModule.agent;
using HolidayFileModule.db.Kiosk;
using HolidayFileModule.entity;
using HolidayFileModule.reader;
using NUnit.Framework;
using RototypeIntl.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HolidayFileModule.UnitTests
{
    [TestFixture]
    class Integration__Reader_HolidayAgentTest
    {
        Reader r;
        string fPath;
        DayOfWeekDataLayer dowdl;
        HolidayCalendarDataLayer hcdl;
        private IDbConnector tempCsdDB;
        private string cp = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\roy\Documents\Visual Studio 2012\Projects\MaestroSVN\HolidayFileModule.UnitTests\database\CSDKiosk.mdb" + ";Persist Security Info=False";
        private bool connected;
        [SetUp]
        public void Setup()
        {
            fPath = @"D:\";
            r = new Reader(fPath);
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
        public void Integration__ReadNBackUp_DataExt_BkExt__ReturnTrue()
        {
            Holiday hol;
            string fullPathToFile = Path.Combine(fPath, "CalendarFile201470.dat");

            Status s = r.ReadNBackUp(out hol, ".dat", ".bak"); //tested at other test file,
            Assert.True(s.Success, s.Message);


            //continous integeration
            s = HolidayAgent.InsertToDayOfWeek(hol.Head, dowdl);
            Assert.True(s.Success, s.Message);

            s = HolidayAgent.InsertToHolidayCalendar(hol.Details, hcdl);
            Assert.True(s.Success, s.Message);

        }

        [TearDown]
        public void TearDown()
        {
            fPath = "";
            r = null;
        }


    }
}
