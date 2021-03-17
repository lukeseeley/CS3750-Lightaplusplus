using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightaplusplus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lightaplusplus.Pages.Courses.Assignments
{
    public class SubmitAssignmentModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public SubmitAssignmentModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        public Users Users { get; set; }

        public int SectionId { get; set; }

        public int AssignmentId { get; set; }

        public Models.Assignments Assignments { get; set; }

        public async Task<IActionResult> OnGetAsync(int sectionId, int? id, int assignmentId)
        {
            SectionId = sectionId;
            AssignmentId = assignmentId;

            if (id == null)
            {
                return RedirectToPage("/Index");
            }

            Assignments = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);
            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            if(Assignments == null)
            {
                return RedirectToPage("/Courses/Index", new { id });
            }

            if (Users == null)
            {
                return RedirectToPage("/Index");
            }

            // TODO: Make Assignment Submittable for student or make page editable for Instructor
            //if (Users.usertype == 'I')
            //{
                
            //}
            //else if (Users.usertype == 'S')
            //{
                
            //}

            return Page();
        }
    }
}
