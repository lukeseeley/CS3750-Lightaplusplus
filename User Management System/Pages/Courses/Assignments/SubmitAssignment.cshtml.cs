using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public Users Users { get; set; }

        public AssignmentSubmissions Submissions { get; set; }

        public int SectionId { get; set; }

        public int AssignmentId { get; set; }

        public Models.Assignments Assignments { get; set; }

        [BindProperty]
        public int HiddenId { get; set; }
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

        public async Task<IActionResult> OnGetAsync(int sectionId, int? id, int assignmentId)
        {
            SectionId = sectionId;
            AssignmentId = assignmentId;
            Success = 0;

            if (id == null)
            {
                return RedirectToPage("/Index");
            }

            Assignments = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);
            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);
            Submissions = await _context.AssignmentSubmissions.FirstOrDefaultAsync(s => s.StudentId == id && s.AssignmentId == assignmentId);

            if(Assignments == null)
            {
                return RedirectToPage("/Courses/Index", new { id });
            }

            if (Users == null)
            {
                return RedirectToPage("/Index");
            }

            try
            {
                if (Submissions != null)
                {
                    Display = false;

                    if(Assignments.AssignmentSubmissionType == 'T')
                    {
                        Assignment = Submissions.Submission;
                    }
                    else if(Assignments.AssignmentSubmissionType == 'F')
                    {
                        FilePath = Submissions.Submission.Substring(0, Submissions.Submission.Length - Users.ID.ToString().Length - Assignments.AssignmentId.ToString().Length - 4) + Submissions.Submission.Substring(Submissions.Submission.Length-4,4);
                    }
                }
                else
                {
                    Display = true;
                }
            }
            catch { }

            this.HiddenId = (int)id;
            this.HiddenAssignmentId = assignmentId;
            this.HiddenSectionId = sectionId;
            return Page();
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            Assignments = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == HiddenAssignmentId);
            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == HiddenId);
            SectionId = HiddenSectionId;
            try
            {
                string path = FileUpload.FileName.Substring(0, FileUpload.FileName.Length - 4) + Users.ID.ToString() + Assignments.AssignmentId.ToString() + FileUpload.FileName.Substring(FileUpload.FileName.Length - 4, 4);
                var file = Path.Combine(_environment.ContentRootPath, "Assignments", path);
                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await FileUpload.CopyToAsync(fileStream);
                }
                AssignmentSubmissions assignment = new AssignmentSubmissions();
                assignment.AssignmentId = HiddenAssignmentId;
                assignment.StudentId = HiddenId;
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
                Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == HiddenId);
                SectionId = HiddenSectionId;

                return Page();
            }

            Success = 1;

            return Page();
        }

        public async Task<IActionResult> OnPostTextAsync()
        {

            try
            {
                AssignmentSubmissions assignment = new AssignmentSubmissions();
                assignment.AssignmentId = HiddenAssignmentId;
                assignment.StudentId = HiddenId;
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
                Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == HiddenId);
                SectionId = HiddenSectionId;

                return Page();
            }

            Success = 1;

            Assignments = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == HiddenAssignmentId);
            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == HiddenId);
            SectionId = HiddenSectionId;

            return Page();
        }

        public async Task<IActionResult> OnPostDownloadFileAsync()
        {
            Assignments = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == HiddenAssignmentId);
            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == HiddenId);
            SectionId = HiddenSectionId;

            Submissions = await _context.AssignmentSubmissions.FirstOrDefaultAsync(s => s.StudentId == HiddenId && s.AssignmentId == HiddenAssignmentId);

            string path = Path.Combine(_environment.ContentRootPath, "Assignments", Submissions.Submission);

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", Submissions.Submission);         
        }
    }
}
