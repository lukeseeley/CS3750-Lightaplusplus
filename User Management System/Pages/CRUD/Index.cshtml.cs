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
    public class IndexModel : PageModel
    {
        private readonly User_Management_System.Data.User_Management_SystemContext _context;

        public IndexModel(User_Management_System.Data.User_Management_SystemContext context)
        {
            _context = context;
        }

        public IList<Users> Users { get;set; }

        public async Task OnGetAsync()
        {
            Users = await _context.Users.ToListAsync();
        }
    }
}
