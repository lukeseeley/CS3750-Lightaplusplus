using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lightaplusplus.Models;

namespace Lightaplusplus.Pages
{
    public class SignUpModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public SignUpModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        [BindProperty, Required(ErrorMessage ="An email address is required"), EmailAddress(ErrorMessage ="Invalid Email Address")]
        [UniqueEmail]
        public string Email { get; set; }

        [BindProperty, Required(ErrorMessage ="You must select what type of user you are.")]
        public string UserType { get; set; }
        public string[] UserTypes = new string[] { "Student", "Instructor" };

        [MinimumAge(18)]
        [BindProperty, Required(ErrorMessage = "A birthday is required")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [BindProperty, Required(ErrorMessage = "You must enter your password.")]
        public string Password { get; set; }

        [BindProperty, Required(ErrorMessage = "You must enter your password.")]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        public Users Users { get; set; }

        [BindProperty]
        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Encryptor encryptor = new Encryptor();
            Users.email = Email;
            Users.birthday = Birthday;

            switch(UserType)
            {
                case ("Student"):
                    Users.usertype = 'S';
                    break;
                case ("Instructor"):
                    Users.usertype = 'I';
                    break;
            }

            // check to see if the passwords match
            if (ConfirmPassword.CompareTo(Password) != 0)
            {
                // if they don't display message and refresh the page
                ErrorMessage = "Passwords do not match.";
                return Page();
            }
            else
            {
                // if they do match set message to empty
                ErrorMessage = string.Empty;
            }

            Users.password = ConfirmPassword;

            //TODO For some reason this has broken, and it thinks the model state is invalid even if it is not
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            Users.password = encryptor.encrypt(Users.password);
            _context.Users.Add(Users);
            await _context.SaveChangesAsync();

            var UserList = await _context.Users.ToListAsync();
            var id = UserList.Last().ID;

            return RedirectToPage("./Welcome", new { id = id});
        }

        
    }

    public class UniqueEmail : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _context = (Data.Lightaplusplus_SystemContext)validationContext.GetService(typeof(Data.Lightaplusplus_SystemContext));
            var email = (string)validationContext.ObjectInstance;
            var existingEmail = _context.Users.Where(u => u.email == email).FirstOrDefault();

            if (existingEmail != null)
            {
                return new ValidationResult("That email is already taken");
            }

            return ValidationResult.Success;
        }
    }

}