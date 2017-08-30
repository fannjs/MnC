using HolidayFileModule.db.Kiosk;
using NUnit.Framework;
using RototypeIntl.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HolidayFileModule.UnitTests
{
    /// <summary>
    /// kiosk version of holiday calendar
    /// </summary>
    [TestFixture]
    public class KioskHolidayCalendarDataLayerTest
    {
        HolidayCalendarDataLayer hcdl;
        private IDbConnector tempCsdDB;
        private string cp = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\roy\Documents\Visual Studio 2012\Projects\MaestroSVN\HolidayFileModule.UnitTests\database\CSDKiosk.mdb" + ";Persist Security Info=False";
        private bool connected;
        
        [SetUp]
        public void Setup()
        {
            connected = SetupDBConn(cp);
            hcdl = new HolidayCalendarDataLayer(tempCsdDB);
        }

        public bool SetupDBConn(string connParam)
        {
            ///warning, to test this piece of codes, if pc: 64bit, use nunit-86.exe, n change ur conf manager to x86 output (so that oledb dll will work)
            tempCsdDB = DatabaseFactory.CreateConnector(DatabaseFactory.DBTYPE.OLEDB, connParam);
            return tempCsdDB.OpenConnection();
        }

        [Test]
        public void ConnDB__ReturnTrue()
        {
            Assert.True(connected, tempCsdDB.Error.GetSimpleError());
        }
        [Test]
        public void Add__ReturnTrue()
        {
            Assert.True(connected, tempCsdDB.Error.GetSimpleError());
        }
    }
}
