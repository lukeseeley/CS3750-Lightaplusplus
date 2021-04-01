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

        public async Task<IActionResult> OnGetAsync(int sectionId)
        {
            var id = Session.getUserId(HttpContext.Session);
            var userType = Session.getUserType(HttpContext.Session);
            ViewData["UserId"] = id;
            ViewData["UserType"] = userType;
            var path = UserValidator.validateUser(_context, HttpContext.Session, 'I');
            if (path != "") return RedirectToPage(path);
            path = UserValidator.validateUser(_context, HttpContext.Session, new KeyPairId("Sec", sectionId));
            if (path != "") return RedirectToPage(path);

            SectionId = sectionId;

            DueDate = DateTime.Today.AddDays(1);
            DueTime = new DateTime().AddHours(12).AddHours(11).AddMinutes(59);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int sectionId)
        {
            var id = Session.getUserId(HttpContext.Session);
            var userType = Session.getUserType(HttpContext.Session);
            ViewData["UserId"] = id;
            ViewData["UserType"] = userType;
            var path = UserValidator.validateUser(_context, HttpContext.Session, 'I');
            if (path != "") return RedirectToPage(path);
            path = UserValidator.validateUser(_context, HttpContext.Session, new KeyPairId("Sec", sectionId));
            if (path != "") return RedirectToPage(path);

            SectionId = sectionId;

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

            return RedirectToPage("/Courses/View/Index", new { sectionId = SectionId });
        }
    }
}
