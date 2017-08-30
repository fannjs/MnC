using HolidayFileModule.db;
using HolidayFileModule.db.ConsoleServer;
using NUnit.Framework;
using RototypeIntl.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HolidayFileModule.UnitTests
{
    [TestFixture]
    public class SpecialDayOfWeekDataLayerTest
    {
        SpecialDayOfWeekDataLayer sdow;
        private IDbConnector tempCsdDB;
        private string cp = "Data Source=126.32.3.39; Integrated Security=false; Initial Catalog=CSDMS;User ID=SSTAuto;Password=1qQA2wWS3eED";

        [SetUp]
        [Ignore]
        public void Setup()
        {
            SetupDBConn(cp);
            sdow = new SpecialDayOfWeekDataLayer(tempCsdDB);
        }



        public bool SetupDBConn(string connParam)
        {
            tempCsdDB = DatabaseFactory.CreateConnector(DatabaseFactory.DBTYPE.SQLDB, connParam);
            return tempCsdDB.OpenConnection();
        }

        [Test]
        [Ignore]
        public void FindDayOfWeekName__When_Data_Exists_In_DB__ReturnsTrue()
        {
            string dowName;
            Status s = sdow.FindDayOfWeekName(out dowName);

            Assert.True(s.Success, s.Message);
        }

        [Test]
        [Ignore]
        public void FindDayOfWeekName__When_Data_Is_Monday__ReturnsDowName()
        {
            string dowName;
            Status s = sdow.FindDayOfWeekName(out dowName);

            //assume record retrieved is Monday
            Assert.AreEqual("Monday", dowName, s.Message);
        }

        [Test]
        [Ignore]
        public void FindBusinessDayList__When_Data_Exists_In_DB__ReturnsTrue()
        {
            List<int> bizzList;
            Status s = sdow.FindBusinessDayList(out bizzList);

            Assert.True(s.Success, s.Message);
        }

        [Test]
        [Ignore]
        public void FindBusinessDayList__When_5businessDays_In_DB__ReturnsFive()
        {
            List<int> bizzList;
            Status s = sdow.FindBusinessDayList(out bizzList);

            Assert.AreEqual(5, bizzList.Count);
        }
    }
}
