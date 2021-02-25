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
        public List<SectionRegistration> SectionRegistrations { get; set; }

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

            SectionsList = await _context.Sections
                .Include(s => s.Instructor)
                .Include(s => s.Course)
                .AsNoTracking()
                .ToListAsync();

            SectionRegistrations = new List<SectionRegistration>();
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
                SectionRegistrations.Add(new SectionRegistration(section, sectionRegistry, registrationStatus));
            }

            return Page();
        }

        public async Task<IActionResult> OnPostRegisterAsync(int studentId, int sectionId)
        {
            SectionStudents sectionStudents = new SectionStudents
            {
                StudentId = studentId,
                SectionId = sectionId
            };

            _context.SectionStudents.Add(sectionStudents);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return RedirectToPage("./Index", new { id = studentId });
        }

        public async Task<IActionResult> OnPostDropAsync(int studentId, int sectionId)
        {
            SectionStudents sectionStudents = new SectionStudents
            {
                StudentId = studentId,
                SectionId = sectionId
            };

            _context.SectionStudents.Remove(sectionStudents);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return RedirectToPage("./Index", new { id = studentId });
        }

    }
    /// <summary>
    /// This is a data class for organizing the information related to a section
    /// </summary>
    public class SectionRegistration
    {
        /// <summary>
        /// This is the Section associated with this section
        /// </summary>
        public Sections Section { get; set; }

        /// <summary>
        /// This is the registry of students related to this section
        /// </summary>
        public List<SectionStudents> StudentRegistry { get; set; }

        /// <summary>
        /// This is the current registration status for this particular section
        /// R -> Registered; F -> Full capacity;  N -> Not registered
        /// </summary>
        public char RegistrationStatus { get; set; }

        public SectionRegistration(Sections section, List<SectionStudents> sectionStudents, char registrationStatus)
        {
            Section = section;
            StudentRegistry = sectionStudents;
            RegistrationStatus = registrationStatus;
        }
    }
}
