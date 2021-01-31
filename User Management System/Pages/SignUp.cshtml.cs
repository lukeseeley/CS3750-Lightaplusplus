using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using User_Management_System.Models;

namespace User_Management_System.Pages
{
    public class SignUpModel : PageModel
    {
        private readonly User_Management_System.Data.User_Management_SystemContext _context;

        public SignUpModel(User_Management_System.Data.User_Management_SystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Users Users { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Encryptor encryptor = new Encryptor();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Users.password = encryptor.encrypt(Users.password);
            _context.Users.Add(Users);
            await _context.SaveChangesAsync();

            var UserList = await _context.Users.ToListAsync();
            var id = UserList.Last().ID;

            return RedirectToPage("./Welcome", new { id = id});
        }
    }
}