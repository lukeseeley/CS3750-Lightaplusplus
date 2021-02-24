using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lightaplusplus.Data;
using Lightaplusplus.Models;

namespace Lightaplusplus.Pages.Registration
{
    public class RegistrationModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public RegistrationModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int StudentId { get; set; }

        [BindProperty]
        public List<Models.Sections> SectionsList { get; set; }

        public string[] Departments = new string[] { "Accounting", "Art", "Biology", "Chemistry", "Computer Science", "Engineering", "English", "Health Science", "History", "Mathematics", "Music", "Social Science", "Physics" };
        

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
            if (user.usertype != 'S') // Make sure only the student is viewing registration
            {
                return RedirectToPage("/Welcome", new { id= id });
            }

            StudentId = (int)id;

            SectionsList = await _context.Sections
                .Include(s => s.Instructor)
                .Include(s => s.Course)
                .AsNoTracking()
                .ToListAsync();

            return Page();
        }
    }
}
