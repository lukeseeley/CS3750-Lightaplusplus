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

        public Users Users { get; set; }

        public SectionStudents SectionStudents { get; set; }

        [BindProperty]
        public int StudentId { get; set; }

        [BindProperty]
        public int SectionId { get; set; }

        [BindProperty]
        public List<Sections> SectionsList { get; set; }

        [BindProperty]
        public List<SectionRegistrationData> SectionRegistrations { get; set; }

        [BindProperty]
        public bool isError { get; set; }

        [BindProperty]
        public string RegisterError { get; set; }

        public string[] Departments = new string[] { "Accounting", "Art", "Biology", "Chemistry", "Computer Science", "Engineering", "English", "Health Science", "History", "Mathematics", "Music", "Social Science", "Physics" };

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            if (user == null)
            {
                return RedirectToPage("/Index");
            }
            if (user.usertype != 'S') // Make sure only the student is viewing registration
            {
                return RedirectToPage("/Welcome", new { id = id });
            }

            StudentId = (int)id;
            Users = user;

            SectionsList = await _context.Sections
                .Include(s => s.Instructor)
                .Include(s => s.Course)
                .AsNoTracking()
                .ToListAsync();

            SectionRegistrations = new List<SectionRegistrationData>();
            foreach (var section in SectionsList)
            {
                var sectionRegistry = await _context.SectionStudents.Where(sr => sr.SectionId == section.SectionId).ToListAsync();
                var isEnrolled = await _context.SectionStudents.Where(ss => ss.SectionId == section.SectionId).Where(ss => ss.StudentId == StudentId).FirstOrDefaultAsync();
                char registrationStatus;
                if (isEnrolled != null) //Meaning this student is already registered in this section
                {
                    registrationStatus = 'R';
                }
                else if (sectionRegistry.Count() >= section.SectionCapacity) //Meaning the class is at full capacity
                {
                    registrationStatus = 'F';
                }
                else //Meaning the student is not enrolled in this class, and the class is not full
                {
                    registrationStatus = 'N';
                }
                SectionRegistrations.Add(new SectionRegistrationData(section, sectionRegistry, registrationStatus));
            }

            isError = false;
            return Page();
        }

        public async Task<IActionResult> OnPostRegisterAsync(int studentId, int sectionId)
        {
            var section = await _context.Sections.FirstOrDefaultAsync(s => s.SectionId == sectionId);
            var register = new StudentRegister(_context);
            var result = register.RegisterStudent(studentId, sectionId);

            if (result == 0)
            {
                isError = false;
                RegisterError = string.Empty;
                return RedirectToPage("./Index", new { id = studentId });
            }
            else isError = true;

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
                    RegisterError = "Unknown Registration Error";
                    break;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDropAsync(int studentId, int sectionId)
        {
            var section = await _context.Sections.FirstOrDefaultAsync(s => s.SectionId == sectionId);
            var register = new StudentRegister(_context);
            var result = register.DropStudent(studentId, sectionId);

            if (result == 0)
            {
                isError = false;
                RegisterError = string.Empty;
                return RedirectToPage("./Index", new { id = studentId });
            }
            else isError = true;

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
                    RegisterError = "Unknown Drop Error";
                    break;
            }

            return Page();
        }

    }

}
