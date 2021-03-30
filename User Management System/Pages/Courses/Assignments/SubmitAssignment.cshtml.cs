using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lightaplusplus.BisLogic;
using Lightaplusplus.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lightaplusplus.Pages.Courses.Assignments
{
    public class SubmitAssignmentModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;
        private IHostingEnvironment _environment;

        public SubmitAssignmentModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public AssignmentSubmissions Submissions { get; set; }

        public int SectionId { get; set; }

        public int AssignmentId { get; set; }

        public Models.Assignments Assignments { get; set; }

        [BindProperty]
        public int HiddenAssignmentId { get; set; }
        [BindProperty]
        public int HiddenSectionId { get; set; }

        [BindProperty]
        public IFormFile FileUpload { get; set; }

        [BindProperty]
        public int Success { get; set; }

        [BindProperty]
        public string Assignment { get; set; }

        public bool Display { get; set; }

        public string FilePath { get; set; }

        [BindProperty]
        public bool Submitted { get; set; }

        public async Task<IActionResult> OnGetAsync(int sectionId, int assignmentId)
        {
            var id = Session.getUserId(HttpContext.Session);
            var userType = Session.getUserType(HttpContext.Session);
            ViewData["UserId"] = id;
            ViewData["UserType"] = userType;
            var path = UserValidator.validateUser(_context, HttpContext.Session, new KeyPairId("Sec", sectionId));
            if (path != "") return RedirectToPage(path);

            SectionId = sectionId;
            AssignmentId = assignmentId;
            Success = 0;

            Assignments = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);
            Submissions = await _context.AssignmentSubmissions.FirstOrDefaultAsync(s => s.StudentId == id && s.AssignmentId == assignmentId);

            if(Assignments == null || Assignments.SectionId != sectionId)
            {
                return RedirectToPage("/Courses/View/Index", new { sectionId });
            }

            try
            {
                if (Submissions != null)
                {
                    Display = false;
                    Submitted = true;

                    if(Assignments.AssignmentSubmissionType == 'T')
                    {
                        Assignment = Submissions.Submission;
                    }
                    else if(Assignments.AssignmentSubmissionType == 'F')
                    {
                        FilePath = Submissions.Submission.Substring(0, Submissions.Submission.Length - id.ToString().Length - Assignments.AssignmentId.ToString().Length - 4) + Submissions.Submission.Substring(Submissions.Submission.Length-4,4);
                    }
                }
                else
                {
                    Display = true;
                    Submitted = false;
                }
            }
            catch { }

            this.HiddenAssignmentId = assignmentId;
            this.HiddenSectionId = sectionId;
            return Page();
        }

        public async Task<IActionResult> OnPostUploadAsync(int sectionId)
        {
            var id = Session.getUserId(HttpContext.Session);
            var redirectpath = UserValidator.validateUser(_context, HttpContext.Session, new KeyPairId("Sec", sectionId));
            if (redirectpath != "") return RedirectToPage(redirectpath);

            Assignments = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == HiddenAssignmentId);
            SectionId = HiddenSectionId;
            try
            {
                string path = FileUpload.FileName.Substring(0, FileUpload.FileName.Length - 4) + id.ToString() + Assignments.AssignmentId.ToString() + FileUpload.FileName.Substring(FileUpload.FileName.Length - 4, 4);
                var file = Path.Combine(_environment.ContentRootPath, "Assignments", path);
                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await FileUpload.CopyToAsync(fileStream);
                }
                AssignmentSubmissions assignment = new AssignmentSubmissions();
                assignment.AssignmentId = HiddenAssignmentId;
                assignment.StudentId = (int)id;
                assignment.SubmissionDateTime = DateTime.Now;
                assignment.Submission = path; 

                _context.AssignmentSubmissions.Add(assignment);
                await _context.SaveChangesAsync();

                Display = false;
                FilePath = FileUpload.FileName;
            }
            catch
            {
                Success = 2;
                Assignments = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == HiddenAssignmentId);
                SectionId = HiddenSectionId;

                return Page();
            }

            Success = 1;

            return Page();
        }

        public async Task<IActionResult> OnPostTextAsync(int sectionId)
        {
            var id = Session.getUserId(HttpContext.Session);
            var path = UserValidator.validateUser(_context, HttpContext.Session, new KeyPairId("Sec", sectionId));
            if (path != "") return RedirectToPage(path);

            try
            {
                AssignmentSubmissions assignment = new AssignmentSubmissions();
                assignment.AssignmentId = HiddenAssignmentId;
                assignment.StudentId = (int)id;
                assignment.SubmissionDateTime = DateTime.Now;
                assignment.Submission = Assignment;

                _context.AssignmentSubmissions.Add(assignment);
                await _context.SaveChangesAsync();

                Display = false;
            }
            catch
            {
                Success = 2;
                Assignments = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == HiddenAssignmentId);
                SectionId = HiddenSectionId;

                return Page();
            }

            Success = 1;

            Assignments = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == HiddenAssignmentId);
            SectionId = HiddenSectionId;

            return Page();
        }

        public async Task<IActionResult> OnPostDownloadFileAsync(int sectionId)
        {
            var id = Session.getUserId(HttpContext.Session);
            var redirectpath = UserValidator.validateUser(_context, HttpContext.Session, new KeyPairId("Sec", sectionId));
            if (redirectpath != "") return RedirectToPage(redirectpath);

            Assignments = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == HiddenAssignmentId);
            SectionId = HiddenSectionId;

            Submissions = await _context.AssignmentSubmissions.FirstOrDefaultAsync(s => s.StudentId == id && s.AssignmentId == HiddenAssignmentId);

            string path = Path.Combine(_environment.ContentRootPath, "Assignments", Submissions.Submission);

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            string fileName = Submissions.Submission.Substring(0, Submissions.Submission.Length - id.ToString().Length - Assignments.AssignmentId.ToString().Length - 4) + Submissions.Submission.Substring(Submissions.Submission.Length - 4, 4);

            return File(bytes, "application/octet-stream", fileName);         
        }
    }
}
