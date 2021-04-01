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
using Newtonsoft.Json;

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


        public void setSectionInfo(int id)
        {
            // get all the sections the student is in
            var StudentSections = _context.SectionStudents.Where(ss => ss.StudentId == id).ToList();

            var SectionsArray = new Sections[StudentSections.Count()];

            // put the sections in a list
            List<Sections> sectionsList = new List<Sections>();
            foreach (var section in StudentSections)
            {
                var sections = _context.Sections.Include(s => s.Instructor).Where(s => s.SectionId == section.SectionId).FirstOrDefault();
                sectionsList.Add(sections);
            }

            // put the list into SectionsArray
            int i = 0;
            foreach (var section in sectionsList)
            {
                SectionsArray[i] = section;
                i++;
            }
            // get the course information
            foreach (var studSection in SectionsArray)
            {
                var courses = _context.Courses.Where(c => c.CourseId == studSection.CourseId);
                foreach (var course in courses)
                {
                    studSection.Course = course;
                }
            }

            foreach (var section in sectionsList)
            {
                var assignments = _context.Assignments.Where(a => a.SectionId == section.SectionId).Include(a => a.Section).ThenInclude(s => s.Course).ToList();

                if (assignments != null)
                {
                    section.Assignments = assignments;
                }

            }
            // Create Cookie in session of sections the user has
            string separatedSections = "";
            foreach (var section in SectionsArray)
            {
                foreach (var assignment in section.Assignments)
                {
                    int courseNo = assignment.Section.Course.CourseNumber;
                    string code = assignment.Section.Course.CourseCode;
                    assignment.Section = new Sections();
                    assignment.Section.Course = new Models.Courses();
                    assignment.Section.Course.CourseNumber = courseNo;
                    assignment.Section.Course.CourseCode = code;
                }
                section.SectionStudents = null;
                section.Course.Sections = null;
                section.Instructor.InstructorSections = null;
                separatedSections = separatedSections + ":::" + JsonConvert.SerializeObject(section);
            }
            Session.setSections(HttpContext.Session, separatedSections);
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var id = Session.getUserId(HttpContext.Session);
            var userType = Session.getUserType(HttpContext.Session);
            ViewData["UserId"] = id;
            ViewData["UserType"] = userType;
            var path = UserValidator.validateUser(_context, HttpContext.Session, 'S');
            if (path != "") return RedirectToPage(path);

            var register = new StudentRegister(_context);
            SectionRegistrations = register.GetSectionRegistration((int)id, HttpContext.Session);

            isError = false;
            // Update session cookie
            setSectionInfo(int.Parse(id.ToString()));
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

            SectionRegistrations = register.GetSectionRegistration((int)id, HttpContext.Session);
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
            // Update session cookie
            setSectionInfo(int.Parse(id.ToString()));
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

            SectionRegistrations = register.GetSectionRegistration((int)id, HttpContext.Session);
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
            // Update session cookie
            setSectionInfo(int.Parse(id.ToString()));
            return Page();
        }
    }
}
