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
using Lightaplusplus.BisLogic;

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

        public int SectionId { get; set; }

        [BindProperty, Required, MaxLength(50)]
        public string AssignmentTitle { get; set; }

        [BindProperty, Required]
        public string AssignmentDescription { get; set; }

        [BindProperty, Required, DataType(DataType.DateTime)]
        public DateTime AssignmentDueDateTime { get; set; }

        [BindProperty, Required, DataType(DataType.Date)]
        public DateTime DueDate {get; set;}

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
        public Models.Assignments Assignments { get; set; }

        [BindProperty]
        public string DueDateError { get; set; }

        [BindProperty]
        public string PointsError { get; set; }

        public async Task<IActionResult> OnGetAsync(int sectionId, int? id)
        {
            SectionId = sectionId;

            if (id == null)
            {
                return RedirectToPage("/Index");
            }

            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            if (Users == null)
            {
                return RedirectToPage("/Index");
            }
            else if(Users.usertype != 'I')
            {
                return RedirectToPage("/Welcome", new { id = id });
            }

            var section = await _context.Sections.FirstOrDefaultAsync(s => s.SectionId == SectionId);

            if (section.InstructorId != Users.ID)
            {
                return RedirectToPage("/Courses/Index", new { id = id });
            }

            DueDate = DateTime.Today.AddDays(1);
            DueTime = new DateTime().AddHours(12).AddHours(11).AddMinutes(59);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int sectionId, int? id)
        {
            SectionId = sectionId;

            if (id == null)
            {
                return RedirectToPage("/Index");
            }

            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            if (Users == null)
            {
                return RedirectToPage("/Index");
            }
            else if (Users.usertype != 'I')
            {
                return RedirectToPage("/Welcome", new { id = id });
            }

            var section = await _context.Sections.FirstOrDefaultAsync(s => s.SectionId == SectionId);

            if (section.InstructorId != Users.ID)
            {
                return RedirectToPage("/Courses/Index", new { id = id });
            }

            // Data validation
            var errors = false;

            if(AssignmentMaxPoints <= 0)
            {
                PointsError = "Please enter a positive amount for points.";
                errors = true;
            }

            if (errors)
            {
                return Page();
            }

            AssignmentAdder myAdder = new AssignmentAdder(_context);
            // Data assignment
            Assignments.SectionId = SectionId;
            Assignments.AssignmentTitle = AssignmentTitle;
            Assignments.AssignmentDescription = AssignmentDescription;
            Assignments.AssignmentDueDateTime = DueDate.Date.Add(DueTime.TimeOfDay);
            Assignments.AssignmentMaxPoints = AssignmentMaxPoints;
            Assignments.AssignmentSubmissionType = AssignmentSubmissionType;

            myAdder.AddAssignment(SectionId, AssignmentTitle, AssignmentDescription, DueDate.Date.Add(DueTime.TimeOfDay), (int)AssignmentMaxPoints, AssignmentSubmissionType);

            _context.Assignments.Add(Assignments);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Courses/View/Index", new { sectionId = SectionId, id = Users.ID });
        }
    }
}
