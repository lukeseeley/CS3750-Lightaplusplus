using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightaplusplus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lightaplusplus.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public ProfileModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Users Users { get; set; }

        [BindProperty]
        public byte[] Image { get; set; }

        [BindProperty]
        public List<UserLinks> Links { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            if (Users == null)
            {
                return NotFound();
            }

            var image = await _context.UserPictures.FirstOrDefaultAsync(p => p.UserID == id);
            Image = image != null ? image.profilepic : null;

            Links = _context.UserLinks.Where(u => u.UserId == (int)id).ToList();

            return Page();
        }
    }
}
