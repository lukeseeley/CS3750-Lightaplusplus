using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lightaplusplus.Models;
using Lightaplusplus.BisLogic;

namespace Lightaplusplus.Pages
{
    public class WelcomeModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public WelcomeModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        public Users Users { get; set; }

        public Sections[] SectionsArray { get; set; }

        public List<Assignments> Assignments { get; set; }

        public List<Assignments> TodoAssignments { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            CreditCard mycard = new CreditCard();

            mycard.cardNumber = "4242424242424242";
            mycard.cvc = "314";
            mycard.exp_month = "2";
            mycard.exp_year = "2022";
            PaymentProcessor.processPayment(mycard, 12.00);
            if (id == null)
            {
                return NotFound();
            }

            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            if (Users == null)
            {
                return RedirectToPage("/Index");
            }

            if (Users.usertype == 'S')
            {
                // get all the sections the student is in
                var StudentSections = await _context.SectionStudents.Where(ss => ss.StudentId == Users.ID).ToListAsync();

                SectionsArray = new Sections[StudentSections.Count()];

                // put the sections in a list
                List<Sections> sectionsList = new List<Sections>();
                foreach (var section in StudentSections)
                {
                    var sections = await _context.Sections.Include(s => s.Instructor).Where(s => s.SectionId == section.SectionId).FirstOrDefaultAsync();
                    sectionsList.Add(sections);
                }

                // put the list into SectionsArray
                int i = 0;
                foreach(var section in sectionsList)
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
            }
            else if (Users.usertype == 'I')
            {
                var sections = _context.Sections.Where(i => i.InstructorId == Users.ID);

                SectionsArray = new Sections[sections.Count()];
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
            }

            Assignments = new List<Assignments>();
            TodoAssignments = new List<Assignments>();

            //Let's get the list of assignments
            if(Users.usertype == 'S')
            {
                var SectionList = await _context.SectionStudents.Where(ss => ss.StudentId == Users.ID).ToListAsync();
                foreach (var section in SectionList)
                {
                    var assignments = await _context.Assignments.Where(a => a.SectionId == section.SectionId).Include(a => a.Section).ThenInclude(s => s.Course).ToListAsync();

                    if (assignments != null)
                    {
                        Assignments.AddRange(assignments);
                    }

                }
            }
            else
            {
                var SectionList = await _context.Sections.Where(s => s.InstructorId == Users.ID).ToListAsync();
                foreach (var section in SectionList)
                {
                    var assignments = await _context.Assignments.Where(a => a.SectionId == section.SectionId).Include(a => a.Section).ThenInclude(s => s.Course).ToListAsync();

                    if (assignments != null)
                    {
                        Assignments.AddRange(assignments);
                    }

                }
            }

            //Now let's created a filtered list for the todo list
            foreach (var assignment in Assignments)
            {
                if (DateTime.Now.CompareTo(assignment.AssignmentDueDateTime) < 0) //The assignment is still in the future
                {
                    if(Users.usertype == 'S')
                    {
                        var Submission = await _context.AssignmentSubmissions.FirstOrDefaultAsync(asub => asub.AssignmentId == assignment.AssignmentId && asub.StudentId == Users.ID);
                        if (Submission != null) continue; //As the student has already submitted this assignment
                    }
                    
                    TodoAssignments.Add(assignment);
                }
            }

            //Now sort to soonest
            TodoAssignments.Sort((a1, a2) => DateTime.Compare(a1.AssignmentDueDateTime, a2.AssignmentDueDateTime));

            if (Users == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}