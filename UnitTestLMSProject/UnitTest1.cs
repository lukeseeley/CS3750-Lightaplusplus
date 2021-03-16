using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lightaplusplus.Pages.Courses;

namespace UnitTestLMSProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void AddCourseTest()
        {
            // Preparation or setup
            CourseAdder myAdder = new CourseAdder();

            // Perform operations
            myAdder.addCourse("CS", 1234, "Test being run", "This is just a test", "Social Sciences", 4);

            // Verify and interpret the results
            Assert.IsTrue(myAdder.checkCourse("CS", 1234));
            Assert.IsFalse(myAdder.checkCourse("CS", 1234));
        }
    }
}
