using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightaplusplus.BisLogic;
using Lightaplusplus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lightaplusplus.Pages.Courses.View
{
    public class IndexModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public IndexModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        public int SectionId { get; set; }


        public Sections Section { get; set; }

        public List<Models.Assignments> Assignments { get; set; }

        public async Task<IActionResult> OnGetAsync(int sectionId)
        {
            var id = Session.getUserId(HttpContext.Session);
            var userType = Session.getUserType(HttpContext.Session);
            ViewData["UserId"] = id;
            ViewData["UserType"] = userType;
            var path = UserValidator.validateUser(_context, HttpContext.Session, new KeyPairId("Sec", sectionId));
            if (path != "") return RedirectToPage(path);

            SectionId = sectionId;

            Section = await _context.Sections.Include(s => s.Course).FirstOrDefaultAsync(s => s.SectionId == SectionId);

            Assignments = await _context.Assignments.Where(a => a.SectionId == SectionId).ToListAsync();

            return Page();
        }
    }
}
