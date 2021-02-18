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

        public Users Users { get; set; }

        public byte[] Image { get; set; }

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

            while (Links.Count < 3)
            {
                var link = new UserLinks();
                link.UserId = (int)id;
                Links.Add(link);
            }

            return Page();
        }
    }
}
