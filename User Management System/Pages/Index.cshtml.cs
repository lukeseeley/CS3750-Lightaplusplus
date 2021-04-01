using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Lightaplusplus.Models;
using Lightaplusplus.BisLogic;
using Newtonsoft.Json;

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

        public void setSectionInfo(int id, char type)
        {
            //var output = JsonConvert.SerializeObject(assignmentEvents);
            if (type == 'S')
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
            else if (type == 'I')
            {
                var sections = _context.Sections.Where(i => i.InstructorId == id);

                var SectionsArray = new Sections[sections.Count()];
                int iter = 0;
                foreach (var section in sections)
                {
                    SectionsArray[iter] = section;
                    iter++;
                }

                foreach (var section in SectionsArray)
                {
                    var courses = _context.Courses.Where(c => c.CourseId == section.CourseId);
                    foreach (var course in courses)
                    {
                        section.Course = course;
                    }
                }
                // Create Cookie in session of sections the user has
                string separatedSections = "";
                foreach (var section in SectionsArray)
                {
                    section.SectionStudents = null; // This has an infinite loop
                    section.Course.Sections = null; // This has an infinite loop
                    section.Instructor.InstructorSections = null; // This has an infinite loop
                    separatedSections = separatedSections + ":::" + JsonConvert.SerializeObject(section);
                }
                Session.setSections(HttpContext.Session, separatedSections);
            }
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
                Session.setUser(HttpContext.Session, User);
                if(User.CurrentLoginTime == null)
                {
                    User.CurrentLoginTime = DateTime.Now;
                }
                else
                {
                    User.LastLoginTime = User.CurrentLoginTime;
                    User.CurrentLoginTime = DateTime.Now;
                }
                Notifications.SetUserObject(HttpContext.Session, "user", User);

                _context.Users.Update(User);
                await _context.SaveChangesAsync();

                setSectionInfo(User.ID, User.usertype);

                return RedirectToPage("./Welcome");
            }

        }
    }
}
