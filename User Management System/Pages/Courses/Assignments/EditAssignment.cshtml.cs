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

        [BindProperty]
        public Models.Assignments Assignments { get; set; }

        public int AssignmentId { get; set; }

        public Users Users { get; set; }
        [BindProperty]
        public int id { get; set; }

        public int SectionId { get; set; }

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

        [BindProperty, Required]
        public virtual Sections Section { get; set; }

        [BindProperty]
        public string DueDateError { get; set; }

        [BindProperty]
        public string PointsError { get; set; }

        [BindProperty]
        public int HiddenId { get; set; }
        [BindProperty]
        public int HiddenAssignmentId { get; set; }
        [BindProperty]
        public int HiddenSectionId { get; set; }

        public async Task<IActionResult> OnGetAsync(int sectionId, int? id, int assignmentId)
        {
            SectionId = sectionId;
            AssignmentId = assignmentId;

            if (id == null)
            {
                return NotFound();
            }

            Assignments = await _context.Assignments
                .Include(a => a.Section).FirstOrDefaultAsync(m => m.AssignmentId == assignmentId);
            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            if (Assignments == null)
            {
                return RedirectToPage("/Courses/Index", new { id });
            }

            if (Users == null)
            {
                return RedirectToPage("/Index");
            }

            DueDate = DateTime.Today.AddDays(1);
            DueTime = new DateTime().AddHours(12).AddHours(11).AddMinutes(59);

            this.id = (int)id;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int sectionId, int? id, int assignmentId)
        {
            Assignments = await _context.Assignments
                .Include(a => a.Section).FirstOrDefaultAsync(m => m.AssignmentId == assignmentId);

            if (!ModelState.IsValid)
            {
                return Page();
            }

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

            return RedirectToPage("/Courses/Assignments/SubmitAssignment", new { sectionId = SectionId, id = Users.ID, assignmentId = AssignmentId});
        }

        private bool AssignmentsExists(int id)
        {
            return _context.Assignments.Any(e => e.AssignmentId == id);
        }
    }
}
