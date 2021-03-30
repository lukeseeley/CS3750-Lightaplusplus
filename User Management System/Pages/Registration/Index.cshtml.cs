using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lightaplusplus.Data;
using Lightaplusplus.Models;
using Lightaplusplus.BisLogic;

namespace Lightaplusplus.Pages.Registration
{
    public class RegistrationModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public RegistrationModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<SectionRegistrationData> SectionRegistrations { get; set; }

        [BindProperty]
        public bool isError { get; set; }

        [BindProperty]
        public string RegisterError { get; set; }

        public string[] Departments = new string[] { "Accounting", "Art", "Biology", "Chemistry", "Computer Science", "Engineering", "English", "Health Science", "History", "Mathematics", "Music", "Social Science", "Physics" };

        public async Task<IActionResult> OnGetAsync()
        {
            var id = Session.getUserId(HttpContext.Session);
            var userType = Session.getUserType(HttpContext.Session);
            ViewData["UserId"] = id;
            ViewData["UserType"] = userType;
            var path = UserValidator.validateUser(_context, HttpContext.Session, 'S');
            if (path != "") return RedirectToPage(path);

            var register = new StudentRegister(_context);
            SectionRegistrations = register.GetSectionRegistration((int)id);

            isError = false;
            return Page();
        }

        public async Task<IActionResult> OnPostRegisterAsync(int sectionId)
        {
            var id = Session.getUserId(HttpContext.Session);
            var userType = Session.getUserType(HttpContext.Session);
            ViewData["UserId"] = id;
            ViewData["UserType"] = userType;

            var path = UserValidator.validateUser(_context, HttpContext.Session, 'S');
            if (path != "") return RedirectToPage(path);

            int studentId = (int)Session.getUserId(HttpContext.Session);
            var register = new StudentRegister(_context);
            var result = register.RegisterStudent(studentId, sectionId);

            if (result == 0)
            {
                return RedirectToPage("./Index");
            }
            else isError = true;

            SectionRegistrations = register.GetSectionRegistration((int)id);
            var section = await _context.Sections.Include(s => s.Course).FirstOrDefaultAsync(s => s.SectionId == sectionId);

            switch (result)
            {
                case (1):
                    RegisterError = "Invalid Student Account.";
                    break;
                case (2):
                    RegisterError = "Invalid Class.";
                    break;
                case (3):
                    RegisterError = "You are already registered in the class: " + section.Course.CourseCode + " " + section.Course.CourseNumber + ".";
                    break;
                case (4):
                    RegisterError = "The class: " + section.Course.CourseCode + " " + section.Course.CourseNumber + " is full.";
                    break;
                default:
                    break;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDropAsync(int sectionId)
        {
            var id = Session.getUserId(HttpContext.Session);
            var userType = Session.getUserType(HttpContext.Session);
            ViewData["UserId"] = id;
            ViewData["UserType"] = userType;

            var path = UserValidator.validateUser(_context, HttpContext.Session, 'S');
            if (path != "") return RedirectToPage(path);

            int studentId = (int)Session.getUserId(HttpContext.Session);
            var register = new StudentRegister(_context);

            var result = register.DropStudent(studentId, sectionId);

            if (result == 0)
            {
                return RedirectToPage("./Index");
            }
            else isError = true;

            SectionRegistrations = register.GetSectionRegistration((int)id);
            var section = await _context.Sections.Include(s => s.Course).FirstOrDefaultAsync(s => s.SectionId == sectionId);

            switch (result)
            {
                case (1):
                    RegisterError = "Invalid Student Account.";
                    break;
                case (2):
                    RegisterError = "Invalid Class.";
                    break;
                case (3):
                    RegisterError = "You are not registered for the course: " + section.Course.CourseCode + " " + section.Course.CourseNumber + ".";
                    break;
                default:
                    break;
            }

            return Page();
        }

    }
}
