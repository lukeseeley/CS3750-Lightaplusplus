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

        [BindProperty]
        public List<string> Departments { get; set; }

        [BindProperty]
        public string ExistingCourseError { get; set; }

        [BindProperty]
        public string CourseError { get; set; }

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

            Departments = new List<string>();
            Departments.Add("Accounting");
            Departments.Add("Art");
            Departments.Add("Biology");
            Departments.Add("Chemistry");
            Departments.Add("Computer Science");
            Departments.Add("Engineering");
            Departments.Add("English");
            Departments.Add("Health Science");
            Departments.Add("History");
            Departments.Add("Mathematics");
            Departments.Add("Music");
            Departments.Add("Social Science");
            Departments.Add("Physics");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
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

            Courses.CourseCode = CourseCode;
            Courses.CourseNumber = CourseNumber;

            Courses.CourseName = CourseName;
            Courses.CourseDescription = CourseDescription;
            Courses.CourseDepartment = CourseDepartment;

            Courses.CourseCreditHours = CourseCreditHours;

            if (errors) return Page();

            _context.Courses.Add(Courses);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Welcome", new { id = id }); //Todo: Redirect to courses overview page instead
        }
    }
}
