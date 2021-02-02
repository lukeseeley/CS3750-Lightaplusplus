using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
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

        [BindProperty, Required(ErrorMessage ="An email address is required"), EmailAddress(ErrorMessage ="Invalid Email Address")]
        public string Email { get; set; }

        [BindProperty, Required(ErrorMessage ="You must select what type of user you are.")]
        public string UserType { get; set; }
        public string[] UserTypes = new string[] { "Student", "Instructor" };

        [BindProperty]
        public Users Users { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Encryptor encryptor = new Encryptor();
            Users.email = Email;

            switch(UserType)
            {
                case ("Student"):
                    Users.usertype = 'S';
                    break;
                case ("Instructor"):
                    Users.usertype = 'I';
                    break;
            }

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