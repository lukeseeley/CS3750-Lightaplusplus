using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Lightaplusplus.BisLogic;
using Lightaplusplus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lightaplusplus.Pages.Courses
{
    public class AddSectionModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public AddSectionModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Users Users { get; set; }
        
        [BindProperty]
        public int InstructorId { get; set; }

        [BindProperty]
        public Sections Sections { get; set; }

        [BindProperty]
        public List<Models.Courses> CourseList { get; set; }

        [BindProperty, Required]
        public string Course { get; set; }

        [BindProperty, Required, MaxLength(250)]
        public string Location { get; set; }

        [BindProperty, DataType(DataType.Time), Required]
        public DateTime StartTime { get; set; }

        [BindProperty, DataType(DataType.Time), Required]
        public DateTime EndTime { get; set; }

        [BindProperty, Required]
        public int Capacity { get; set; }

        [BindProperty]
        public bool isOnMonday { get; set; }
        [BindProperty]
        public bool isOnTuesday { get; set; }
        [BindProperty]
        public bool isOnWednesday { get; set; }
        [BindProperty]
        public bool isOnThursday { get; set; }
        [BindProperty]
        public bool isOnFriday { get; set; }
        [BindProperty]
        public bool isOnSaturday { get; set; }
        [BindProperty]
        public bool isOnSunday { get; set; }

        [BindProperty]
        public string CourseError { get; set; }
        [BindProperty]
        public string DaysError { get; set; }
        [BindProperty]
        public string CapacityError { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var id = Session.getUserId(HttpContext.Session);
            var userType = Session.getUserType(HttpContext.Session);
            ViewData["UserId"] = id;
            ViewData["UserType"] = userType;
            var path = UserValidator.validateUser(_context, HttpContext.Session, 'I');
            if (path != "") return RedirectToPage(path);

            InstructorId = (int)id;
            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            CourseList = await _context.Courses.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnGetSearch(string courseSearch)
        {
            var path = UserValidator.validateUser(_context, HttpContext.Session, 'I');
            if (path != "") return RedirectToPage(path);

            List<string> courseList = new List<string>();
            CourseList = await _context.Courses.ToListAsync();

            foreach (var course in CourseList)
            {
                courseList.Add(course.CourseCode + " " + course.CourseNumber);
            }

            return new JsonResult(courseList);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var id = Session.getUserId(HttpContext.Session);
            var path = UserValidator.validateUser(_context, HttpContext.Session, 'I');
            if (path != "") return RedirectToPage(path);

            Sections.InstructorId = (int)id;

            //Course Check
            int CourseId = 0;
            var course = Course.Split(" ");
            var errors = false;
            if(course.Length < 2)
            {
                CourseError = "Please enter a valid Course Code and Number";
                errors = true;
            }
            else
            {
                int courseNumber;
                if(Int32.TryParse(course[1], out courseNumber) == false)
                {
                    CourseError = "Please enter a valid Course Number";
                    errors = true;
                }
                else
                {
                    var resultCourse = await _context.Courses.Where(c => c.CourseCode == course[0]).Where(c => c.CourseNumber == courseNumber).FirstOrDefaultAsync();
                    if (resultCourse == null)
                    {
                        CourseError = "That Course does not exist";
                        errors = true;
                    }
                    else
                    {
                        CourseError = string.Empty;
                        CourseId = resultCourse.CourseId;
                    }
                }
            }
            //Capacity check
            if (Capacity < 1)
            {
                CapacityError = "Capacity must be at least 1";
                errors = true;
            }
            else CapacityError = string.Empty;

            Sections.CourseId = CourseId;
            Sections.SectionCapacity = Capacity;


            //Assign Days Taught
            string daysTaught = "";
            if (isOnMonday) daysTaught += "M";
            if (isOnTuesday) daysTaught += "T";
            if (isOnWednesday) daysTaught += "W";
            if (isOnThursday) daysTaught += "H";
            if (isOnFriday) daysTaught += "F";
            if (isOnSaturday) daysTaught += "S";
            if (isOnSunday) daysTaught += "U";

            if (daysTaught == string.Empty)
            {
                DaysError = "You must teach at least one day a week";
                errors = true;
            }
            else DaysError = string.Empty;

            if (errors) return Page();

            Sections.DaysTaught = daysTaught;

            //Validate Times
            if(StartTime < EndTime) //Is the starting time earlier than the ending time
            {
                Sections.SectionTimeStart = StartTime;
                Sections.SectionTimeEnd = EndTime;
            }
            else //If not, simply flip it around for the user
            {
                Sections.SectionTimeStart = EndTime;
                Sections.SectionTimeEnd = StartTime;
            }

            //General assignment
            Sections.SectionLocation = Location;

            _context.Sections.Add(Sections);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Courses");
        }
    }
}
