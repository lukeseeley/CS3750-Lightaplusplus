using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightaplusplus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lightaplusplus.Pages.Courses
{
    public class AddSectionModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public AddSectionModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int InstructorId { get; set; }

        [BindProperty]
        public Sections Sections { get; set; }

        [BindProperty]
        public List<Models.Courses> CourseList { get; set; }

        [BindProperty]
        public string Course { get; set; }

        [BindProperty]
        public bool isOnMonday { get; set; }
        [BindProperty]
        public bool isOnTuesday { get; set; }
        [BindProperty]
        public bool isOnWednesday { get; set; }
        [BindProperty]
        public bool isOnThursday { get; set; }
        [BindProperty]
        public bool isOnFriday { get; set; }
        [BindProperty]
        public bool isOnSaturday { get; set; }
        [BindProperty]
        public bool isOnSunday { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
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
                return RedirectToPage("/Welcome", new { id = id }); //Todo: Redirect to courses overview page instead
            }

            InstructorId = (int)id;

            CourseList = await _context.Courses.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnGetSearch(string courseSearch)
        {
            List<string> courseList = new List<string>();
            CourseList = await _context.Courses.ToListAsync();

            foreach (var course in CourseList)
            {
                courseList.Add(course.CourseCode + " " + course.CourseNumber);
            }

            return new JsonResult(courseList);
        }
    }
}
