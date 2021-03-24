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
using System.ComponentModel.DataAnnotations;

namespace Lightaplusplus.Pages.Courses.Assignments
{
    public class EditAssignmentModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public EditAssignmentModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        public Users Users { get; set; }

        public int id { get; set; }

        public Sections Section { get; set; }

        public int SectionId { get; set; }

        [BindProperty]
        public Models.Assignments Assignments { get; set; }

        public int AssignmentId { get; set; }

        [BindProperty, Required, MaxLength(50)]
        public string AssignmentTitle { get; set; }

        [BindProperty, Required]
        public string AssignmentDescription { get; set; }

        [BindProperty, Required, DataType(DataType.DateTime)]
        public DateTime AssignmentDueDateTime { get; set; }

        [BindProperty, Required, DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [BindProperty, Required, DataType(DataType.Time)]
        public DateTime DueTime { get; set; }

        [BindProperty, Required]
        public int? AssignmentMaxPoints { get; set; }

        /// <summary>
        /// The submission type allowed for this assignment
        /// Types include: F -> File submission; T -> Text Submission
        /// </summary>
        [BindProperty, Required]
        public char AssignmentSubmissionType { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int sectionId, int assignmentId)
        {

            if (id == null)
            {
                return NotFound();
            }

            Assignments = await _context.Assignments
                .Include(a => a.Section).FirstOrDefaultAsync(m => m.AssignmentId == assignmentId);
            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);
            Section = await _context.Sections.FirstOrDefaultAsync(s => s.SectionId == sectionId);

            if (Users == null)
            {
                return RedirectToPage("/Index");
            }
            else if (Users.usertype != 'I')
            {
                return RedirectToPage("/Welcome", new { id = id });
            }

            if (Assignments == null)
            {
                return NotFound();
            }

            if (Section.InstructorId != Users.ID)
            {
                return RedirectToPage("/Courses/Index", new { id = id });
            }

            AssignmentTitle = Assignments.AssignmentTitle;
            AssignmentDescription = Assignments.AssignmentDescription;
            AssignmentDueDateTime = Assignments.AssignmentDueDateTime;
            AssignmentMaxPoints = Assignments.AssignmentMaxPoints;
            AssignmentSubmissionType = Assignments.AssignmentSubmissionType;
            DueDate = Assignments.AssignmentDueDateTime.Date;
            DueTime = DateTime.Parse(Assignments.AssignmentDueDateTime.ToString());
            DueTime.ToString("HH:mm tt");

            this.id = (int)id;
            AssignmentId = assignmentId;
            SectionId = sectionId;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id, int sectionId, int assignmentId)
        {
            Assignments = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);
            Assignments.AssignmentTitle = AssignmentTitle;
            Assignments.AssignmentDescription = AssignmentDescription;
            Assignments.AssignmentDueDateTime = DueDate.Date.Add(DueTime.TimeOfDay);
            Assignments.AssignmentMaxPoints = AssignmentMaxPoints;
            Assignments.AssignmentSubmissionType = AssignmentSubmissionType;


            _context.Attach(Assignments).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssignmentsExists(Assignments.AssignmentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Courses/Assignments/SubmitAssignment", new { id, sectionId, assignmentId});
        }

        private bool AssignmentsExists(int assignmentId)
        {
            return _context.Assignments.Any(e => e.AssignmentId == assignmentId);
        }
    }
}
