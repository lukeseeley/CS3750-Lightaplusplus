using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lightaplusplus.Models;
using System.ComponentModel.DataAnnotations;

namespace Lightaplusplus.Pages.Courses
{
    public class AddCourseModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public AddCourseModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Users Users { get; set; }

        [BindProperty]
        public int id { get; set; }

        [BindProperty]
        public Models.Courses Courses { get; set; }

        [BindProperty, Required, MaxLength(10)]
        public string CourseCode { get; set; }

        [BindProperty, Required]
        public int CourseNumber { get; set; }

        [BindProperty, Required, MaxLength(50)]
        public string CourseName { get; set; }

        [BindProperty, Required]
        public string CourseDescription { get; set; }

        [BindProperty, Required]
        public int CourseCreditHours { get; set; }

        [BindProperty, Required]
        public string CourseDepartment { get; set; }

        public string[] Departments = new string[] { "Accounting", "Art", "Biology", "Chemistry", "Computer Science", "Engineering", "English", "Health Science", "History", "Mathematics", "Music", "Social Science", "Physics" };

        [BindProperty]
        public string ExistingCourseError { get; set; }

        [BindProperty]
        public string CourseError { get; set; }

        [BindProperty]
        public string CreditError { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(id == null)
            {
                return RedirectToPage("/Index");
            }
            
            var user = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            if (user == null)
            {
                return RedirectToPage("/Index");
            }
            if (user.usertype != 'I') //Ensure that only an instructor can add a new course
            {
                return RedirectToPage("/Welcome", new { id = id }); //Todo: Redirect to courses overview page instead
            }

            this.id = (int)id;
            CourseCreditHours = 3;
            Users = user;

            return Page();
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
            _context.Courses.Add(Courses);
            _context.SaveChanges();
            var testc = checkCourse(CourseCode, CourseNumber);
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {   
            //Handle security checks first
            if (id == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            if (user == null)
            {
                return RedirectToPage("/Index");
            }
            if (user.usertype != 'I') //Ensure that only an instructor can add a new course
            {
                return RedirectToPage("/Welcome", new { id = id });
            }

            //Handle Validation checks
            var errors = false;
            var existingCourse = await _context.Courses.Where(c => c.CourseCode == CourseCode).Where(c => c.CourseNumber == CourseNumber).FirstOrDefaultAsync();
            if(existingCourse != null)
            {
                ExistingCourseError = "That Course already exists.";
                errors = true;
            }
            else ExistingCourseError = string.Empty;

            if(CourseNumber < 100)
            {
                CourseError = "Please enter a valid Course Number";
                errors = true;
            } 
            else CourseError = string.Empty;

            if (CourseCreditHours < 0)
            {
                CreditError = "You must enter a positive number of credits";
                errors = true;
            }
            else CreditError = string.Empty;

            if (errors) return Page();

            /*Courses.CourseCode = CourseCode;
            Courses.CourseNumber = CourseNumber;

            Courses.CourseName = CourseName;
            Courses.CourseDescription = CourseDescription;
            Courses.CourseDepartment = CourseDepartment;

            Courses.CourseCreditHours = CourseCreditHours;


            _context.Courses.Add(Courses);*/

            addCourse(CourseCode, CourseNumber, CourseName, CourseDescription, CourseDepartment, CourseCreditHours);
            //await _context.SaveChangesAsync();

            return RedirectToPage("/Courses/Index", new { id = id });
        }
    }
}
