using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Lightaplusplus.Data;
using Lightaplusplus.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lightaplusplus.Pages.Courses
{
    public class AddAssignmentModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public AddAssignmentModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        public Users Users { get; set; }

        [BindProperty]
        public int SectionId { get; set; }

        [BindProperty]
        public int InstructorId { get; set; }

        [BindProperty, Required, MaxLength(50)]
        public string AssignmentTitle { get; set; }

        [BindProperty, Required]
        public string AssignmentDescription { get; set; }

        [BindProperty, Required, DataType(DataType.DateTime)]
        public DateTime AssignmentDueDateTime { get; set; }

        [BindProperty, Required]
        public int? AssignmentMaxPoints { get; set; }

        /// <summary>
        /// The submission type allowed for this assignment
        /// Types include: F -> File submission; T -> Text Submission
        /// </summary>
        [BindProperty, Required]
        public char AssignmentSubmissionType { get; set; }

        [BindProperty, Required]
        public virtual Sections Section { get; set; }

        [BindProperty]
        public Assignments Assignments { get; set; }

        [BindProperty]
        public bool IsError { get; set; }

        [BindProperty]
        public string AssignmentsError { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int sectionId)
        {
            if (id == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);
            var section = await _context.Sections.FirstOrDefaultAsync(s => s.SectionId == sectionId);

            if (user == null)
            {
                return RedirectToPage("/Index");
            }
            if (user.usertype != 'I') //Ensure that only an instructor can add a new assignment
            {
                return RedirectToPage("/Welcome", new { id = id }); //Todo: Redirect to courses overview page instead
            }
            if(section == null)
            {
                return RedirectToPage("/Index");
            }

            InstructorId = (int)id;
            SectionId = (int)sectionId;
            Users = user;



            IsError = false;
            //ViewData["SectionId"] = new SelectList(_context.Sections, "SectionId", "DaysTaught");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Assignments.Add(Assignments);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
