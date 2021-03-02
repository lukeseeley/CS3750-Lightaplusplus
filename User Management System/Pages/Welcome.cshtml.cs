using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lightaplusplus.Models;

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

            if (Users == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}