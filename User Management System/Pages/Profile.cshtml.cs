using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightaplusplus.BisLogic;
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

        [BindProperty]
        public Notifications Notifications { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var id = Session.getUserId(HttpContext.Session);
            var userType = Session.getUserType(HttpContext.Session);
            ViewData["UserId"] = id;
            ViewData["UserType"] = userType;
            var path = UserValidator.validateUser(_context, HttpContext.Session);
            if (path != "") return RedirectToPage(path);

            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            var image = await _context.UserPictures.FirstOrDefaultAsync(p => p.UserID == id);
            Image = image != null ? image.profilepic : null;

            Links = _context.UserLinks.Where(u => u.UserId == (int)id).ToList();

            if ((string)ViewData["UserType"] == "S")
            {
                Notifications = new Notifications(HttpContext.Session, _context);
            }

            return Page();
        }
    }
}
