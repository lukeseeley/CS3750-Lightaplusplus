using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lightaplusplus.Models;
using System.ComponentModel.DataAnnotations;
using Lightaplusplus.BisLogic;
using Newtonsoft.Json;

namespace Lightaplusplus.Pages
{
    public class CourseOverviewModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public CourseOverviewModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int id { get; set; }

        [BindProperty]
        public Sections[] sectionsTaught { get; set; }

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

        public async Task<IActionResult> OnGetAsync()
        {
            var id = Session.getUserId(HttpContext.Session);
            var userType = Session.getUserType(HttpContext.Session);
            ViewData["UserId"] = id;
            ViewData["UserType"] = userType;
            var path = UserValidator.validateUser(_context, HttpContext.Session, 'I');
            if (path != "") return RedirectToPage(path);

            sectionsTaught = Session.getSections(HttpContext.Session).ToArray();
            return Page();
        }
    }
}
