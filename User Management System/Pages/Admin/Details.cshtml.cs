using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using User_Management_System.Data;
using User_Management_System.Models;

namespace User_Management_System.Pages.CRUD
{
    public class DetailsModel : PageModel
    {
        private readonly User_Management_System.Data.User_Management_SystemContext _context;

        public DetailsModel(User_Management_System.Data.User_Management_SystemContext context)
        {
            _context = context;
        }

        public Users Users { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            if (Users == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
