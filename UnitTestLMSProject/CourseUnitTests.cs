using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lightaplusplus.Pages.Courses;
using Microsoft.EntityFrameworkCore;
using Lightaplusplus.Models;
using System.ComponentModel.DataAnnotations;
using Lightaplusplus.BisLogic;

namespace UnitTestLMSProject
{
    [TestClass]
    public class CourseUnitTests
    {
        [TestMethod]
        public void AddCourseTest()
        {
            // Preparation or setup
            var options = new DbContextOptionsBuilder<Lightaplusplus.Data.Lightaplusplus_SystemContext>();
            options.UseSqlServer(UnitTestConfig.ConnectionString);
            var context = new Lightaplusplus.Data.Lightaplusplus_SystemContext(options.Options);
            CourseAdder myAdder = new CourseAdder(context);

            // Perform operations
            myAdder.addCourse("CS", 123456789, "Test being run", "This is just a test", "Social Sciences", 4);

            // Analyze results
            Assert.IsTrue(myAdder.checkCourse("CS", 123456789));
            Assert.IsFalse(myAdder.checkCourse("CS", 123456788)); // Make sure it's not just telling us everything is true
        }
        [TestMethod]
        public void RemoveCourseTest()
        {
            // Preparation or setup
            var options = new DbContextOptionsBuilder<Lightaplusplus.Data.Lightaplusplus_SystemContext>();
            options.UseSqlServer(UnitTestConfig.ConnectionString);
            var context = new Lightaplusplus.Data.Lightaplusplus_SystemContext(options.Options);
            CourseAdder myAdder = new CourseAdder(context);

            // Perform operations
            myAdder.removeCourse("CS", 123456789);

            // Analyze results
            Assert.IsFalse(myAdder.checkCourse("CS", 123456789));
        }
    }
}
