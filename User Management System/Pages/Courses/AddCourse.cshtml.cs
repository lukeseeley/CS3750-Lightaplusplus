using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lightaplusplus.Models;

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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(id == null)
            {
                return RedirectToPage("./Index");
            }
            
            var user = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);
            
            if(user.usertype != 'I') //Ensure that only an instructor can add a new course
            {
                return RedirectToPage("./Welcome", new { id = id }); //Todo: Redirect to courses overview page instead
            }

            this.id = (int)id;

            return Page();
        }
    }
}
