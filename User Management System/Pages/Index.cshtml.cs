using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Lightaplusplus.Models;

namespace Lightaplusplus.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public IndexModel(ILogger<IndexModel> logger, Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public Users Users { get; set; }

        [BindProperty, Required(ErrorMessage = "An email address is required"), EmailAddress(ErrorMessage = "Please enter a valid Email Address")]
        public string Email { get; set; }

        [BindProperty, Required(ErrorMessage = "You must enter your password.")]
        public string Password { get; set; }

        [BindProperty]
        public string ErrorMessage { get; set; }

        public void OnGet(int? result)
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //TODO For some reason this is currently broken
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            Encryptor encryptor = new Encryptor();

            string password = encryptor.encrypt(Password);

            var User = await _context.Users.Where(u => u.email == Email).Where(u => u.password == password).FirstOrDefaultAsync();

            if (User == null)
            {
                ErrorMessage = "Either the email or password you entered was incorrect";
                return Page();
            } 
            else
            {
                return RedirectToPage("./Welcome", new { id = User.ID });
            }

        }
    }
}
