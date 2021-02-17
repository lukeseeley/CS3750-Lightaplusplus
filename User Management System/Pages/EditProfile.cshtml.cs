using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lightaplusplus.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace Lightaplusplus.Pages
{
    public class EditProfileModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public EditProfileModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        public Users Users { get; set; }

        [BindProperty]
        public int id { get; set; }

        [BindProperty, Required]
        public string Firstname { get; set; }

        [BindProperty, Required]
        public string Lastname { get; set; }

        //[MinimumAge(18)]
        [BindProperty, Required(ErrorMessage = "A birthday is required")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [BindProperty, Phone(ErrorMessage ="Please enter a valid phone number"), MinLength(10, ErrorMessage = "Please enter a valid phone number")]
        public string Phonenumber { get; set; }

        [BindProperty]
        public string Addressline1 { get; set; }

        [BindProperty]
        public string Addressline2 { get; set; }

        [BindProperty]
        public string Addresscity { get; set; }

        [BindProperty]
        public string Addressstate { get; set; }

        [BindProperty]
        public int Addresszip { get; set; }

        [BindProperty]
        public string Bio { get; set; }

        public IFormFile FileUpload { get; set; }

        public byte[] Image { get; set; }

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

            Firstname = Users.firstname ;
            Lastname = Users.lastname;
            Birthday = Users.birthday;
            Phonenumber = Users.phonenumber;
            Addressline1 = Users.addressline1;
            Addressline2 = Users.addressline2;
            Addresscity = Users.addresscity;
            Addressstate = Users.addressstate;
            Addresszip = Users.addresszip;
            Bio = Users.bio;

            //byte[] byteArray = Users.Picture.profilepic;
            //Image = new FileContentResult(byteArray, "image/jpeg");

            this.id = (int)id;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);
            Users.firstname = Firstname;
            Users.lastname = Lastname;
            Users.birthday = Birthday;
            Users.phonenumber = Phonenumber;
            Users.addressline1 = Addressline1;
            Users.addressline2 = Addressline2;
            Users.addresscity = Addresscity;
            Users.addressstate = Addressstate;
            Users.addresszip = Addresszip;
            Users.bio = Bio;

            Image = Users.Picture.profilepic;

            _context.Attach(Users).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(Users.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Profile", new { id = id });
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);


            using (var memoryStream = new MemoryStream())
            {
                await FileUpload.CopyToAsync(memoryStream);

                // Upload the file if less than 2 MB
                if (memoryStream.Length < 2097152)
                {
                    if (Users.Picture == null)
                    {
                        UserPictures picture = new UserPictures();
                        picture.UserID = Users.ID;
                        picture.User = Users;
                        picture.profilepic = memoryStream.ToArray();
                        Users.Picture = picture;
                        Image = memoryStream.ToArray();
                        return Page();
                    }
                    else
                    {
                        Users.Picture.profilepic = memoryStream.ToArray();
                    }

                    _context.Attach(Users).State = EntityState.Modified;

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UsersExists(Users.ID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("File", "The file is too large.");
                }
            }

            return RedirectToPage("./Profile", new { id = id });
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.ID == id);
        }
    }
}
