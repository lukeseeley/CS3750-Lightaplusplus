using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using User_Management_System.Models;

namespace User_Management_System.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly User_Management_System.Data.User_Management_SystemContext _context;

        public IndexModel(ILogger<IndexModel> logger, User_Management_System.Data.User_Management_SystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public Users Users { get; set; }

        public void OnGet(int? result)
        {
            if (result == -1)
            {
                ViewData["error"] = "The email you entered does not exist or you entered the wrong password";
            } 
            else
            {
                ViewData["error"] = "";
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Encryptor encryptor = new Encryptor();

            string email = Users.email;
            string password = encryptor.encrypt(Users.password);

            var User = await _context.Users.Where(u => u.email == email).Where(u => u.password == password).FirstOrDefaultAsync();

            if (User == null)
            {
                return RedirectToPage("./Index", new { result = -1 });
            } 
            else
            {
                return RedirectToPage("./Welcome", new { id = User.ID });
            }

        }

        public int checkUser(string email, string password)
        {
        var user = from u in _context.Users
                   where u.email.Equals(email)
                   select u;

        var validUser = _context.Users.Where(u => u.password == password).FirstOrDefault();

        if (validUser == null)
        {
            return -1;
        }
        else
        {
            return validUser.ID;
        }
        }
    }
}
