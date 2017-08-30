using HolidayFileModule.entity;
using HolidayFileModule.reader;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HolidayFileModule.UnitTests
{
    [TestFixture]
    public class ReaderActionTest
    {
        string fPath;

        [SetUp]
        public void Setup()
        {
            fPath = @"D:\";
        }

        [Test]
        [Ignore]
        public void Read__ValidFilePath__ReturnTrue()
        {
            Holiday hol;
            string fullPathToFile = Path.Combine(fPath, "CalendarFile201470.dat");

            Status s = ReaderAction.ReadHoliday(out hol, fullPathToFile, ReaderAction.ReadHolidayHeader, ReaderAction.ReadHolidayDetail);
            Assert.True(s.Success, s.Message);

        }


        [Test]
     
        public void Read__InvalidFilePath__ReturnFalse()
        {
            Holiday hol;

            Status s = ReaderAction.ReadHoliday(out hol, @"C:\", ReaderAction.ReadHolidayHeader, ReaderAction.ReadHolidayDetail);
            Assert.False(s.Success, s.Message);
        }
    }
}
