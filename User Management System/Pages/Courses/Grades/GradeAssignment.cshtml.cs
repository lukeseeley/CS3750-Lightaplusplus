using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lightaplusplus.BisLogic;
using Lightaplusplus.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lightaplusplus.Pages.Courses.Grades
{
    public class GradeAssignmentModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;
        private IHostingEnvironment _environment;

        public GradeAssignmentModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public AssignmentSubmissions Submissions { get; set; }

        [BindProperty]
        public Models.Grades Grade { get; set; }

        [BindProperty]
        public int GradeValue { get; set; }

        [BindProperty]
        public string GradeValueError { get; set; }

        [BindProperty]
        public bool Success { get; set; }

        [BindProperty]
        public bool Graded { get; set; }

        public int SectionId { get; set; }

        public int AssignmentId { get; set; }

        public int SubmissionId { get; set; }

        public string FilePath { get; set; }

        [BindProperty]
        public int HiddenSubmissionId { get; set; }

        public async Task<IActionResult> OnGetAsync(int sectionId, int assignmentId, int submissionId)
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
            AssignmentId = assignmentId;
            SubmissionId = submissionId;

            Success = false;
            Graded = false;

            var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if(assignment == null)
            {
                return RedirectToPage("/Courses/Index");
            }

            Submissions = await _context.AssignmentSubmissions.Include(@as => @as.Assignment).ThenInclude(a => a.Section).FirstOrDefaultAsync(@as => @as.SubmissionId == submissionId);

            if (Submissions == null)
            {
                return RedirectToPage("/Courses/Grades/Submissions", new { sectionId = sectionId, assignmentId = assignmentId });
            }

            if(Submissions.Assignment.AssignmentSubmissionType == 'F')
            {
                FilePath = Submissions.Submission.Substring(0, Submissions.Submission.Length - Submissions.StudentId.ToString().Length - Submissions.Assignment.AssignmentId.ToString().Length - 4) + Submissions.Submission.Substring(Submissions.Submission.Length - 4, 4);
            }

            Grade = await _context.Grades.Where(g => g.AssignmentId == AssignmentId).FirstOrDefaultAsync(g =>g.StudentId == Submissions.StudentId);

            if (Grade != null)
            {
                Graded = true;
                GradeValue = Grade.GradeValue;
            }

            HiddenSubmissionId = Submissions.SubmissionId;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int sectionId, int assignmentId, int submissionId)
        {
            var id = Session.getUserId(HttpContext.Session);
            var path = UserValidator.validateUser(_context, HttpContext.Session, 'I');
            if (path != "") return RedirectToPage(path);
            path = UserValidator.validateUser(_context, HttpContext.Session, new KeyPairId("Sec", sectionId));
            if (path != "") return RedirectToPage(path);

            SectionId = sectionId;
            AssignmentId = assignmentId;
            SubmissionId = submissionId;

            var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null)
            {
                return RedirectToPage("/Courses/Index");
            }

            Submissions = await _context.AssignmentSubmissions.Include(@as => @as.Assignment).ThenInclude(a => a.Section).FirstOrDefaultAsync(@as => @as.SubmissionId == submissionId);


            if (Submissions == null)
            {
                return RedirectToPage("/Courses/Grades/Submissions");
            }

            if (Submissions.Assignment.AssignmentSubmissionType == 'F')
            {
                FilePath = Submissions.Submission.Substring(0, Submissions.Submission.Length - Submissions.StudentId.ToString().Length - Submissions.Assignment.AssignmentId.ToString().Length - 4) + Submissions.Submission.Substring(Submissions.Submission.Length - 4, 4);
            }

            //Check for Errors
            bool errors = false;

            if (GradeValue < 0)
            {
                GradeValueError = "Please enter a non negative value.";
                errors = true;
            }
            else if (GradeValue > Submissions.Assignment.AssignmentMaxPoints)
            {
                GradeValueError = $"Please enter a number no more than {Submissions.Assignment.AssignmentMaxPoints}.";
                errors = true;
            }
            else GradeValueError = string.Empty;

            if(errors)
            {
                return Page();
            }

            Grade.AssignmentId = Submissions.AssignmentId;
            Grade.StudentId = Submissions.StudentId;
            Grade.GradeDateTime = DateTime.Now;
            Grade.GradeValue = GradeValue;

            _context.Grades.Add(Grade);
            await _context.SaveChangesAsync();

            Success = true;

            return Page();
        }

        public async Task<IActionResult> OnPostDownloadFileAsync(int sectionId)
        {
            var id = Session.getUserId(HttpContext.Session);
            var redirectpath = UserValidator.validateUser(_context, HttpContext.Session, 'I');
            if (redirectpath != "") return RedirectToPage(redirectpath);
            redirectpath = UserValidator.validateUser(_context, HttpContext.Session, new KeyPairId("Sec", sectionId));
            if (redirectpath != "") return RedirectToPage(redirectpath);

            SubmissionId = HiddenSubmissionId;

            Submissions = await _context.AssignmentSubmissions.Include(@as => @as.Assignment).FirstOrDefaultAsync(s => s.SubmissionId == SubmissionId);

            string path = Path.Combine(_environment.ContentRootPath, "Assignments", Submissions.Submission);

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            string fileName = Submissions.Submission.Substring(0, Submissions.Submission.Length - Submissions.StudentId.ToString().Length - Submissions.Assignment.AssignmentId.ToString().Length - 4) + Submissions.Submission.Substring(Submissions.Submission.Length - 4, 4);

            return File(bytes, "application/octet-stream", fileName);
        }
    }
}
