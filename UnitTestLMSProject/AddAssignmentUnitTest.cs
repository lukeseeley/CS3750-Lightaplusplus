using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lightaplusplus.Pages.Courses;
using Microsoft.EntityFrameworkCore;
using Lightaplusplus.Models;
using System.ComponentModel.DataAnnotations;
using Lightaplusplus.BisLogic;
using System;

namespace UnitTestLMSProject
{
    [TestClass]
    public class AddAssignmentUnitTest
    {
        [TestMethod]
        public void AddAssignmentTest()
        {
            // Preparation or setup
            var options = new DbContextOptionsBuilder<Lightaplusplus.Data.Lightaplusplus_SystemContext>();
            options.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=LMS_ELON;User ID=LMS_ELON;Password=$Y02X9iwsdAQ3HcDPUig");
            var context = new Lightaplusplus.Data.Lightaplusplus_SystemContext(options.Options);
            AssignmentAdder myAdder = new AssignmentAdder(context);

            // Perform operations
            myAdder.AddAssignment(1006, "Midterm 1", "First Test", DateTime.Now, 600, 'T');

            // Analyze results
            Assert.IsTrue(myAdder.CheckAssignment("Midterm 1", 1006));
            Assert.IsFalse(myAdder.CheckAssignment("Midterm 2", 1006)); // Make sure it's not just telling us everything is true
        }

        [TestMethod]
        public void RemoveAssignmentTest()
        {
            // Preparation or setup
            var options = new DbContextOptionsBuilder<Lightaplusplus.Data.Lightaplusplus_SystemContext>();
            options.UseSqlServer(UnitTestConfig.ConnectionString);
            var context = new Lightaplusplus.Data.Lightaplusplus_SystemContext(options.Options);
            AssignmentAdder myAdder = new AssignmentAdder(context);

            // Perform operations
            myAdder.RemoveAssignmet("Midterm 1", 1006);

            // Analyze results
            Assert.IsFalse(myAdder.CheckAssignment("Midterm 1", 1006));
        }
    }
}
