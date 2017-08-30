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
    class ReaderTest
    {
        Reader r;
        string fPath;

        [SetUp]
        public void Setup()
        {
            fPath = @"D:\";
            r = new Reader(fPath);
        }


        [Test]
        public void ReadNBackUp__DataExt__BkExt__ReturnTrue()
        {
            Holiday hol;
            string fullPathToFile = Path.Combine(fPath, "CalendarFile201470.dat");

            Status s = r.ReadNBackUp(out hol, ".dat", ".bak");

            Assert.True(s.Success, s.Message);

            Assert.True(File.Exists(Path.Combine(fPath,".bak")), ".bak shall be created");
        }

        [TearDown]
        public void TearDown()
        {
            fPath = "";
            r = null;
        }
    }
}
