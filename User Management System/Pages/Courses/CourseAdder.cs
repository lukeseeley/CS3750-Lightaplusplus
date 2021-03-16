using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightaplusplus.Models;
using System.ComponentModel.DataAnnotations;

namespace Lightaplusplus.Pages.Courses
{
    public class CourseAdder
    {

        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;
        public CourseAdder(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        public void addCourse(string CourseCode, int CourseNumber, string CourseName, string CourseDescription, string CourseDepartment, int CourseCreditHours)
        {
            Models.Courses newCourse = new Models.Courses();

            newCourse.CourseCode = CourseCode;
            newCourse.CourseNumber = CourseNumber;
            newCourse.CourseName = CourseName;
            newCourse.CourseDescription = CourseDescription;
            newCourse.CourseDepartment = CourseDepartment;
            newCourse.CourseCreditHours = CourseCreditHours;
            _context.Courses.Add(newCourse);
            _context.SaveChanges();
            //var testc = checkCourse(CourseCode, CourseNumber);
            return;
        }

        public bool checkCourse(string CourseCode, int CourseNumber)
        {
            var course = _context.Courses.Where(c => c.CourseCode == CourseCode).Where(c => c.CourseNumber == CourseNumber).First();

            return course != null;
        }

        public void removeCourse(string CourseCode, int CourseNumber)
        {
            _context.Courses.Remove(_context.Courses.Where(c => c.CourseCode == CourseCode).Where(c => c.CourseNumber == CourseNumber).First());
            return;
        }
    }
}
