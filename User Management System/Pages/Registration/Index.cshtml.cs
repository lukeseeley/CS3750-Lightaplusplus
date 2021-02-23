using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public IList<Sections> Sections { get; set; }

        public async Task OnGetAsync()
        {
            Sections = await _context.Sections
                .Include(s => s.Instructor)
                .Include(s => s.Course)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
