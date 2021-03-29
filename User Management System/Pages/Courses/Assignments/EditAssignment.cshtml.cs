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
using Lightaplusplus.BisLogic;

namespace Lightaplusplus.Pages.Courses.Assignments
{
    public class EditAssignmentModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public EditAssignmentModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

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

        public async Task<IActionResult> OnGetAsync(int sectionId, int assignmentId)
        {
            var id = Session.getUserId(HttpContext.Session);
            var userType = Session.getUserType(HttpContext.Session);
            ViewData["UserId"] = id;
            ViewData["UserType"] = userType;
            var path = UserValidator.validateUser(_context, HttpContext.Session, 'I');
            if (path != "") return RedirectToPage(path);
            path = UserValidator.validateUser(_context, HttpContext.Session, new KeyPairId("Sec", sectionId));
            if (path != "") return RedirectToPage(path);

            Assignments = await _context.Assignments
                .Include(a => a.Section).FirstOrDefaultAsync(m => m.AssignmentId == assignmentId);
            Section = await _context.Sections.FirstOrDefaultAsync(s => s.SectionId == sectionId);

            if (Assignments == null)
            {
                return NotFound();
            }

            AssignmentTitle = Assignments.AssignmentTitle;
            AssignmentDescription = Assignments.AssignmentDescription;
            AssignmentDueDateTime = Assignments.AssignmentDueDateTime;
            AssignmentMaxPoints = Assignments.AssignmentMaxPoints;
            AssignmentSubmissionType = Assignments.AssignmentSubmissionType;
            DueDate = Assignments.AssignmentDueDateTime.Date;
            DueTime = DateTime.Parse(Assignments.AssignmentDueDateTime.ToString());
            DueTime.ToString("HH:mm tt");

            AssignmentId = assignmentId;
            SectionId = sectionId;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int sectionId, int assignmentId)
        {
            var id = Session.getUserId(HttpContext.Session);
            var path = UserValidator.validateUser(_context, HttpContext.Session, 'I');
            if (path != "") return RedirectToPage(path);
            path = UserValidator.validateUser(_context, HttpContext.Session, new KeyPairId("Sec", sectionId));
            if (path != "") return RedirectToPage(path);

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

            return RedirectToPage("/Courses/Assignments/SubmitAssignment", new {sectionId, assignmentId});
        }

        private bool AssignmentsExists(int assignmentId)
        {
            return _context.Assignments.Any(e => e.AssignmentId == assignmentId);
        }
    }
}
