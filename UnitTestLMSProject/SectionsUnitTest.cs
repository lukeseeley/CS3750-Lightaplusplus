using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Lightaplusplus.BisLogic;

namespace UnitTestLMSProject
{
    [TestClass]
    public class SectionsUnitTest
    {
        [TestMethod]
        public void AddSectionTest()
        {
            // Preparation or setup
            var options = new DbContextOptionsBuilder<Lightaplusplus.Data.Lightaplusplus_SystemContext>();
            options.UseSqlServer(UnitTestConfig.ConnectionString);
            var context = new Lightaplusplus.Data.Lightaplusplus_SystemContext(options.Options);
            SectionAdder myAdder = new SectionAdder(context);
            DateTime time = new DateTime(2012, 12, 25, 10, 30, 50);

            // Perform operations
            myAdder.addSection(1054, 13, "Room 103", time, time.AddHours(1), "MWF", 30);

            // Analyze results
            Assert.IsTrue(myAdder.checkSection(1054, 13, time));
            Assert.IsFalse(myAdder.checkSection(1055, 14, time)); // Make sure it's not just telling us everything is true
        }
        [TestMethod]
        public void RemoveSectionTest()
        {
            // Preparation or setup
            var options = new DbContextOptionsBuilder<Lightaplusplus.Data.Lightaplusplus_SystemContext>();
            options.UseSqlServer(UnitTestConfig.ConnectionString);
            var context = new Lightaplusplus.Data.Lightaplusplus_SystemContext(options.Options);
            SectionAdder myAdder = new SectionAdder(context);
            DateTime time = new DateTime(2012, 12, 25, 10, 30, 50);

            // Perform operations
            myAdder.removeSection(1054, 13, time);

            // Analyze results
            Assert.IsFalse(myAdder.checkSection(1054, 13, time));
        }
    }
}
