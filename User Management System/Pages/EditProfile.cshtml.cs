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
using System.Text.RegularExpressions;
using Lightaplusplus.BisLogic;

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

        [BindProperty]
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
        public string phoneErrorMessage { get; set; }

        [BindProperty]
        public string zipErrorMessage { get; set; }

        [BindProperty]
        public string Bio { get; set; }

        [BindProperty]
        public IFormFile FileUpload { get; set; }

        [BindProperty]
        public byte[] Image { get; set; }
        [BindProperty]
        public string PictureErrorMessage { get; set; }
        
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
            var image = await _context.UserPictures.FirstOrDefaultAsync(p => p.UserID == id);
            Image = image != null ? image.profilepic : null;

            Links = _context.UserLinks.Where(u => u.UserId == (int)id).ToList();

            while(Links.Count < 3)
            {
                var link = new UserLinks();
                link.UserId = (int)id;
                Links.Add(link);
            }

            if ((string)ViewData["UserType"] == "S")
            {
                Notifications = new Notifications(HttpContext.Session, _context);
            }

            this.id = (int)id;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var path = UserValidator.validateUser(_context, HttpContext.Session);
            if (path != "") return RedirectToPage(path);

            bool notValid = false;
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

            var img = await _context.UserPictures.FirstOrDefaultAsync(p => p.UserID == Users.ID);
            Image = img.profilepic;

            var contextChanged = false;
            foreach (var link in Links)
            {
                if(link.LinkId != 0 && link.link == null)
                {
                    link.UserId = Users.ID;
                    link.User = Users;
                    _context.UserLinks.Remove(link);
                    contextChanged = true;

                }
                else if (link.LinkId != 0 && link.link != null)
                {
                    link.UserId = Users.ID;
                    link.User = Users;
                    Users.Links.Add(link);
                    contextChanged = true;
                }
                else if (link.LinkId == 0 && link.link != null)
                {
                    link.UserId = Users.ID;
                    link.User = Users;
                    Users.Links.Add(link);
                    contextChanged = true;
                }
            }
            if (contextChanged)
            {
                await _context.SaveChangesAsync();
            }

            if (!Regex.IsMatch(Addresszip.ToString(), "[\\d-]{5,}") && Addresszip.ToString() != "" && Addresszip.ToString() != "0")
            {
                zipErrorMessage = "Invalid Zipcode";
                notValid = true;
            }
            else
            {
                // if they do match set message to empty
                zipErrorMessage = string.Empty;
            }

            if (Phonenumber != null && !Regex.IsMatch(Phonenumber, "^(\\d{10,}|[0 - 9 -]{10,}|[0 - 9\\.]{10,}|[0 - 9\\s]{10,}$)"))
            {
 
                phoneErrorMessage = "Invalid Phone Number.";
                notValid = true;
            }
            else
            {
                // if they do match set message to empty
                phoneErrorMessage = string.Empty;
            }

            if (notValid)
            {
                notValid = false;
                return Page();
            }

            if ((string)ViewData["UserType"] == "S")
            {
                Notifications = new Notifications(HttpContext.Session, _context);
            }

            _context.Attach(Users).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Profile");
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            var path = UserValidator.validateUser(_context, HttpContext.Session);
            if (path != "") return RedirectToPage(path);

            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            var image = await _context.UserPictures.FirstOrDefaultAsync(p => p.UserID == id);

            if ((string)ViewData["UserType"] == "S")
            {
                Notifications = new Notifications(HttpContext.Session, _context);
            }

            // check to see if the field is blank
            if (FileUpload != null)
            {
                // check file type
                if (FileUpload.ContentType == "image/jpeg" || FileUpload.ContentType == "image/png")
                {

                    using (var memoryStream = new MemoryStream())
                    {
                        await FileUpload.CopyToAsync(memoryStream);

                        // Upload the file if less than 2 MB
                        if (memoryStream.Length < 2097152)
                        {
                            // check to see if a user already has a profile picture
                            if (image == null)
                            {
                                // create a picture
                                UserPictures picture = new UserPictures();
                                picture.UserID = Users.ID;
                                picture.User = Users;
                                picture.profilepic = memoryStream.ToArray();
                                Users.Picture = picture;
                            }
                            else
                            {
                                // update profile pic
                                image.profilepic = memoryStream.ToArray();
                            }

                            PictureErrorMessage = string.Empty;

                            _context.Attach(Users).State = EntityState.Modified;

                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            PictureErrorMessage = "File size is too big.  It needs to be less than 2 MB.";
                            Image = Users.Picture.profilepic;
                            return Page();
                        }
                    }

                    return RedirectToPage("./EditProfile");
                }
                else
                {
                    PictureErrorMessage = "Incorrect file type.";
                    Image = Users.Picture.profilepic;
                    return Page();
                }
            }
            else
            {
                PictureErrorMessage = "No image is selected.";
                Image = Users.Picture.profilepic;
                return Page();
            }
        }
    }
}
