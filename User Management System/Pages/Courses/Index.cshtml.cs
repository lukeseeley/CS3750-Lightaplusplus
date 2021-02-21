using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lightaplusplus.Models;
using System.ComponentModel.DataAnnotations;

namespace Lightaplusplus.Pages
{
    public class CourseOverviewModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public CourseOverviewModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        public Users Users { get; set; }

        [BindProperty]
        public int id { get; set; }

        [BindProperty]
        public Sections[] sectionsTaught { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("/Index");
            }

            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            if (Users == null)
            {
                return RedirectToPage("/Index");
            }
            if (Users.usertype != 'I') //Ensure that only an instructor can add a new course
            {
                return RedirectToPage("/Welcome", new { id = id });
            }
            var sections = _context.Sections.Where(i => i.InstructorId == Users.ID);

            sectionsTaught = new Sections[sections.Count()];
            int iter = 0;
            foreach (var section in sections)
            {
                sectionsTaught[iter] = section;
                iter++;
            }

            foreach (var section in sectionsTaught)
            {
                var courses = _context.Courses.Where(c => c.CourseId == section.CourseId);
                foreach (var course in courses)
                {
                    section.Course = course;
                }
            }
            return Page();
        }
    }
}
