using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lightaplusplus.Models;
using Lightaplusplus.BisLogic;
using Newtonsoft.Json;

namespace Lightaplusplus.Pages
{
    public class WelcomeModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public WelcomeModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        public Users Users { get; set; }

        public Sections[] SectionsArray { get; set; }

        public List<Assignments> Assignments { get; set; }

        public Event[] assignmentEvents { get; set; }

        public RecurringEvent[] sectionEvents { get; set; }

        public List<Assignments> TodoAssignments { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            if (Users == null)
            {
                return RedirectToPage("/Index");
            }

            if (Users.usertype == 'S')
            {
                // get all the sections the student is in
                var StudentSections = await _context.SectionStudents.Where(ss => ss.StudentId == Users.ID).ToListAsync();

                SectionsArray = new Sections[StudentSections.Count()];

                // put the sections in a list
                List<Sections> sectionsList = new List<Sections>();
                foreach (var section in StudentSections)
                {
                    var sections = await _context.Sections.Include(s => s.Instructor).Where(s => s.SectionId == section.SectionId).FirstOrDefaultAsync();
                    sectionsList.Add(sections);
                }

                // put the list into SectionsArray
                int i = 0;
                foreach(var section in sectionsList)
                {
                    SectionsArray[i] = section;
                    i++;
                }
                // get the course information
                foreach (var studSection in SectionsArray)
                {
                    var courses = _context.Courses.Where(c => c.CourseId == studSection.CourseId);
                    foreach (var course in courses)
                    {
                        studSection.Course = course;
                    }
                }
            }
            else if (Users.usertype == 'I')
            {
                var sections = _context.Sections.Where(i => i.InstructorId == Users.ID);

                SectionsArray = new Sections[sections.Count()];
                int iter = 0;
                foreach (var section in sections)
                {
                    SectionsArray[iter] = section;
                    iter++;
                }

                foreach (var section in SectionsArray)
                {
                    var courses = _context.Courses.Where(c => c.CourseId == section.CourseId);
                    foreach (var course in courses)
                    {
                        section.Course = course;
                    }
                }
            }

            Assignments = new List<Assignments>();
            TodoAssignments = new List<Assignments>();

            //Let's get the list of assignments
            if(Users.usertype == 'S')
            {
                var SectionList = await _context.SectionStudents.Where(ss => ss.StudentId == Users.ID).ToListAsync();
                foreach (var section in SectionList)
                {
                    var assignments = await _context.Assignments.Where(a => a.SectionId == section.SectionId).Include(a => a.Section).ThenInclude(s => s.Course).ToListAsync();

                    if (assignments != null)
                    {
                        Assignments.AddRange(assignments);
                    }

                }
            }
            else
            {
                var SectionList = await _context.Sections.Where(s => s.InstructorId == Users.ID).ToListAsync();
                foreach (var section in SectionList)
                {
                    var assignments = await _context.Assignments.Where(a => a.SectionId == section.SectionId).Include(a => a.Section).ThenInclude(s => s.Course).ToListAsync();

                    if (assignments != null)
                    {
                        Assignments.AddRange(assignments);
                    }

                }
            }

            //Now let's created a filtered list for the todo list
            foreach (var assignment in Assignments)
            {
                if (DateTime.Now.CompareTo(assignment.AssignmentDueDateTime) < 0) //The assignment is still in the future
                {
                    if(Users.usertype == 'S')
                    {
                        var Submission = await _context.AssignmentSubmissions.FirstOrDefaultAsync(asub => asub.AssignmentId == assignment.AssignmentId && asub.StudentId == Users.ID);
                        if (Submission != null) continue; //As the student has already submitted this assignment
                    }
                    
                    TodoAssignments.Add(assignment);
                }
            }



            if (Users.usertype == 'S')
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

                // get all the sections the student is in
                var StudentSections = await _context.SectionStudents.Where(ss => ss.StudentId == Users.ID).ToListAsync();

                SectionsArray = new Sections[StudentSections.Count()];

                // put the sections in a list
                List<Sections> sectionsList = new List<Sections>();
                foreach (var section in StudentSections)
                {
                    var sections = await _context.Sections.Include(s => s.Instructor).Where(s => s.SectionId == section.SectionId).FirstOrDefaultAsync();
                    sectionsList.Add(sections);
                }

                // put the list into SectionsArray
                int i = 0;
                foreach (var section in sectionsList)
                {
                    SectionsArray[i] = section;
                    i++;
                }
                // get the course information
                foreach (var studSection in SectionsArray)
                {
                    var courses = _context.Courses.Where(c => c.CourseId == studSection.CourseId);
                    foreach (var course in courses)
                    {
                        studSection.Course = course;
                    }
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
            else if (Users.usertype == 'I')
            {
                assignmentEvents = new Event[0]; // Teachers don't have assignments listed on their calendar
                var sections = _context.Sections.Where(i => i.InstructorId == Users.ID);

                SectionsArray = new Sections[sections.Count()];
                int iter = 0;
                foreach (var section in sections)
                {
                    SectionsArray[iter] = section;
                    iter++;
                }

                foreach (var section in SectionsArray)
                {
                    var courses = _context.Courses.Where(c => c.CourseId == section.CourseId);
                    foreach (var course in courses)
                    {
                        section.Course = course;
                    }
                }

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

            //Now sort to soonest
            TodoAssignments.Sort((a1, a2) => DateTime.Compare(a1.AssignmentDueDateTime, a2.AssignmentDueDateTime));

            if (Users == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}