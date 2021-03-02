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
        public string TitleError { get; set; }

        [BindProperty]
        public string DescriptionError { get; set; }

        [BindProperty]
        public string DueDateError { get; set; }

        [BindProperty]
        public string PointsError { get; set; }

        public async Task<IActionResult> OnGetAsync(int? sectionId)
        {
            if (sectionId == null)
            {
                return RedirectToPage("/Courses/Index", new { InstructorId });
            }

            var section = await _context.Sections.FirstOrDefaultAsync(s => s.SectionId == sectionId);

            SectionId = (int)sectionId;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? sectionId)
        {
            if (sectionId == null)
            {
                return RedirectToPage("/Courses/Index", new { InstructorId });
            }

            var section = await _context.Sections.FirstOrDefaultAsync(s => s.SectionId == sectionId);

            SectionId = (int)sectionId;

            // Data validation
            var errors = false;

            if(AssignmentTitle == string.Empty)
            {
                TitleError = "Please enter an assignment title.";
                errors = true;
            }
            else if(AssignmentTitle.Length > 50)
            {
                TitleError = "Please enter a title that's less than 50 characters.";
                errors = true;
            }

            if(AssignmentDescription == string.Empty)
            {
                DescriptionError = "Please enter an assignment description.";
                errors = true;
            }

            if(AssignmentDueDateTime == null)
            {
                DueDateError = "Please enter a due date for the assignment.";
                errors = true;
            }

            if(AssignmentMaxPoints <= 0)
            {
                PointsError = "Please enter a positive amount for points.";
                errors = true;
            }

            if (errors)
            {
                return Page();
            }


            // Data assignment
            Assignments.SectionId = SectionId;
            Assignments.AssignmentTitle = AssignmentTitle;
            Assignments.AssignmentDescription = AssignmentDescription;
            Assignments.AssignmentDueDateTime = AssignmentDueDateTime;
            Assignments.AssignmentMaxPoints = AssignmentMaxPoints;
            Assignments.AssignmentSubmissionType = AssignmentSubmissionType;

            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            _context.Assignments.Add(Assignments);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Courses/Index", new { InstructorId });
        }
    }
}
