using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lightaplusplus.Models;
using Newtonsoft.Json;
using Lightaplusplus.BisLogic;

namespace Lightaplusplus.Pages
{
    public class CalendarModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public CalendarModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        public Sections[] SectionsArray { get; set; }

        public List<Assignments> Assignments { get; set; }

        public Event[] assignmentEvents { get; set; }

        public RecurringEvent[] sectionEvents { get; set; }

        public List<Assignments> TodoAssignments { get; set; }

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

            if (id == null)
            {
                return NotFound();
            }

            SectionsArray = Session.getSections(HttpContext.Session).ToArray();
            Assignments = Session.getAssignments(HttpContext.Session);
            TodoAssignments = new List<Assignments>();

            //Now let's created a filtered list for the todo list
            foreach (var assignment in Assignments)
            {
                if (DateTime.Now.CompareTo(assignment.AssignmentDueDateTime) < 0) //The assignment is still in the future
                {
                    if (userType == "S")
                    {
                        var Submission = await _context.AssignmentSubmissions.FirstOrDefaultAsync(asub => asub.AssignmentId == assignment.AssignmentId && asub.StudentId == id); // *****REMOVE REQUEST******
                        if (Submission != null) continue; //As the student has already submitted this assignment
                    }

                    TodoAssignments.Add(assignment);
                }
            }

            if (userType == "S")
            {
                assignmentEvents = new Event[Assignments.Count()];
                for (int b = 0; b < Assignments.Count(); ++b)
                {
                    var myEvent = new Event();
                    myEvent.title = Assignments[b].AssignmentTitle;
                    myEvent.start = Assignments[b].AssignmentDueDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    myEvent.end = Assignments[b].AssignmentDueDateTime.AddSeconds(1).ToString("yyyy-MM-dd HH:mm:ss");
                    myEvent.link = "/Courses/" + Assignments[b].SectionId + "/Assignments/" + Assignments[b].AssignmentId;
                    // "/Courses/" + sectionid + "/Assignments/" + assignmentid
                    assignmentEvents[b] = myEvent;
                }

                // create classes
                sectionEvents = new RecurringEvent[SectionsArray.Length];
                for (int j = 0; j < SectionsArray.Length; j++)
                {
                    RecurringEvent myEvent = new RecurringEvent();
                    myEvent.title = SectionsArray[j].Course.CourseCode + " " + SectionsArray[j].Course.CourseNumber.ToString();
                    myEvent.startTime = SectionsArray[j].SectionTimeStart.ToString("HH:mm:ss");
                    myEvent.endTime = SectionsArray[j].SectionTimeEnd.ToString("HH:mm:ss");
                    myEvent.description = SectionsArray[j].Course.CourseDescription;
                    List<int> numericalDays = new List<int>();
                    foreach (var letter in SectionsArray[j].DaysTaught)
                    {
                        switch (letter)
                        {
                            case 'M':
                                numericalDays.Add(1);
                                break;
                            case 'T':
                                numericalDays.Add(2);
                                break;
                            case 'W':
                                numericalDays.Add(3);
                                break;
                            case 'H':
                                numericalDays.Add(4);
                                break;
                            case 'F':
                                numericalDays.Add(5);
                                break;
                            case 'S':
                                numericalDays.Add(6);
                                break;
                            case 'U':
                                numericalDays.Add(7);
                                break;
                        }
                    }

                    myEvent.daysOfWeek = new int[numericalDays.Count()];
                    for (int k = 0; k < numericalDays.Count(); ++k)
                    {
                        myEvent.daysOfWeek[k] = numericalDays[k];
                    }
                    sectionEvents[j] = myEvent;
                }
            }
            else if (userType == "I")
            {
                assignmentEvents = new Event[0]; // Teachers don't have assignments listed on their calendar
                sectionEvents = new RecurringEvent[SectionsArray.Length];
                for (int j = 0; j < SectionsArray.Length; j++)
                {
                    RecurringEvent myEvent = new RecurringEvent();
                    myEvent.title = SectionsArray[j].Course.CourseCode + " " + SectionsArray[j].Course.CourseNumber.ToString();
                    myEvent.startTime = SectionsArray[j].SectionTimeStart.ToString("HH:mm:ss");
                    myEvent.endTime = SectionsArray[j].SectionTimeEnd.ToString("HH:mm:ss");
                    myEvent.description = SectionsArray[j].Course.CourseDescription;
                    List<int> numericalDays = new List<int>();
                    foreach (var letter in SectionsArray[j].DaysTaught)
                    {
                        switch (letter)
                        {
                            case 'M':
                                numericalDays.Add(1);
                                break;
                            case 'T':
                                numericalDays.Add(2);
                                break;
                            case 'W':
                                numericalDays.Add(3);
                                break;
                            case 'H':
                                numericalDays.Add(4);
                                break;
                            case 'F':
                                numericalDays.Add(5);
                                break;
                            case 'S':
                                numericalDays.Add(6);
                                break;
                            case 'U':
                                numericalDays.Add(7);
                                break;
                        }
                    }

                    myEvent.daysOfWeek = new int[numericalDays.Count()];
                    for (int k = 0; k < numericalDays.Count(); ++k)
                    {
                        myEvent.daysOfWeek[k] = numericalDays[k];
                    }
                    sectionEvents[j] = myEvent;
                }
            }

            if ((string)ViewData["UserType"] == "S")
            {
                Notifications = new Notifications(HttpContext.Session, _context);
            }

            return Page();
        }
    }
}
