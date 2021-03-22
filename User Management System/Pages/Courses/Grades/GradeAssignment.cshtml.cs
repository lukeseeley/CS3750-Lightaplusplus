using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public Users Users { get; set; }

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
        public int HiddenId { get; set; }
        [BindProperty]
        public int HiddenSubmissionId { get; set; }

        public async Task<IActionResult> OnGetAsync(int sectionId, int? id, int assignmentId, int submissionId)
        {
            SectionId = sectionId;
            AssignmentId = assignmentId;
            SubmissionId = submissionId;

            Success = false;
            Graded = false;

            if (id == null)
            {
                return RedirectToPage("/Index");
            }

            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            if(Users.usertype != 'I')
            {
                return RedirectToPage("/Welcome", new { id = id });
            }

            var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if(assignment == null)
            {
                return RedirectToPage("/Courses/Index", new { id = id});
            }

            Submissions = await _context.AssignmentSubmissions.Include(@as => @as.Assignment).ThenInclude(a => a.Section).FirstOrDefaultAsync(@as => @as.SubmissionId == submissionId);

            if (Submissions == null)
            {
                return RedirectToPage("/Courses/Grades/Submissions", new { id = id, sectionId = sectionId, assignmentId = assignmentId });
            }

            if(Submissions.Assignment.Section.InstructorId != Users.ID)
            {
                return RedirectToPage("/Courses/Index", new { id = id });
            }

            if(Submissions.Assignment.AssignmentSubmissionType == 'F')
            {
                FilePath = Submissions.Submission.Substring(0, Submissions.Submission.Length - Users.ID.ToString().Length - Submissions.Assignment.AssignmentId.ToString().Length - 4) + Submissions.Submission.Substring(Submissions.Submission.Length - 4, 4);
            }

            Grade = await _context.Grades.Where(g => g.AssignmentId == AssignmentId).FirstOrDefaultAsync(g =>g.StudentId == Submissions.StudentId);

            if (Grade != null)
            {
                Graded = true;
                GradeValue = Grade.GradeValue;
            }

            HiddenId = Submissions.StudentId;
            HiddenSubmissionId = Submissions.SubmissionId;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int sectionId, int? id, int assignmentId, int submissionId)
        {
            SectionId = sectionId;
            AssignmentId = assignmentId;
            SubmissionId = submissionId;

            if (id == null)
            {
                return RedirectToPage("/Index");
            }

            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            if (Users.usertype != 'I')
            {
                return RedirectToPage("/Welcome", new { id = id });
            }

            var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null)
            {
                return RedirectToPage("/Courses/Index", new { id = id });
            }

            Submissions = await _context.AssignmentSubmissions.Include(@as => @as.Assignment).ThenInclude(a => a.Section).FirstOrDefaultAsync(@as => @as.SubmissionId == submissionId);


            if (Submissions == null)
            {
                return RedirectToPage("/Courses/Grades/Submissions", new { id = id, sectionId = sectionId, assignmentId = assignmentId });
            }

            if (Submissions.Assignment.Section.InstructorId != Users.ID)
            {
                return RedirectToPage("/Courses/Index", new { id = id });
            }

            if (Submissions.Assignment.AssignmentSubmissionType == 'F')
            {
                FilePath = Submissions.Submission.Substring(0, Submissions.Submission.Length - Users.ID.ToString().Length - Submissions.Assignment.AssignmentId.ToString().Length - 4) + Submissions.Submission.Substring(Submissions.Submission.Length - 4, 4);
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

        public async Task<IActionResult> OnPostDownloadFileAsync()
        {
            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == HiddenId);
            SubmissionId = HiddenSubmissionId;

            Submissions = await _context.AssignmentSubmissions.Include(@as => @as.Assignment).FirstOrDefaultAsync(s => s.SubmissionId == SubmissionId);

            string path = Path.Combine(_environment.ContentRootPath, "Assignments", Submissions.Submission);

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            string fileName = Submissions.Submission.Substring(0, Submissions.Submission.Length - Users.ID.ToString().Length - Submissions.Assignment.AssignmentId.ToString().Length - 4) + Submissions.Submission.Substring(Submissions.Submission.Length - 4, 4);

            return File(bytes, "application/octet-stream", fileName);
        }
    }
}
