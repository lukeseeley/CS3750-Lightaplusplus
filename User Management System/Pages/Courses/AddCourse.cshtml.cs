using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lightaplusplus.Models;
using System.ComponentModel.DataAnnotations;
using Lightaplusplus.BisLogic;

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

        public async Task<IActionResult> OnGetAsync()
        {
            var id = Session.getUserId(HttpContext.Session);
            var userType = Session.getUserType(HttpContext.Session);
            ViewData["UserId"] = id;
            ViewData["UserType"] = userType;
            var path = UserValidator.validateUser(_context, HttpContext.Session, 'I');
            if (path != "") return RedirectToPage(path);

            this.id = (int)id;
            CourseCreditHours = 3;
            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var id = Session.getUserId(HttpContext.Session);
            var path = UserValidator.validateUser(_context, HttpContext.Session, 'I');
            if (path != "") return RedirectToPage(path);

            //Handle Validation checks
            var errors = false;
            var existingCourse = await _context.Courses.Where(c => c.CourseCode == CourseCode).Where(c => c.CourseNumber == CourseNumber).FirstOrDefaultAsync();
            CourseAdder myAdder = new CourseAdder(_context);
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

            myAdder.addCourse(CourseCode, CourseNumber, CourseName, CourseDescription, CourseDepartment, CourseCreditHours);

            return RedirectToPage("/Courses/Index");
        }
    }
}
