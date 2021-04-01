using Lightaplusplus.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.BisLogic
{
    public class Notifications
    {
        public List<Notification> NotificationList { get; set; }

        public Notifications(ISession session, Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            var id = Session.getUserId(session);
            var usertype = Session.getUserType(session);
            var currentLogin = Session.getUserCurrentLogin(session);
            var lastLogin = Session.getUserLastLogin(session);
            var sections = Session.getAllSections(session);

            if (lastLogin == null)
            {
                lastLogin = DateTime.Parse("1/1/1980");
            }

            NotificationList = new List<Notification>();

            if(usertype == "S")
            {
                foreach (var section in sections)
                {
                    var GradeList = context.Grades.Include(g => g.Assignment).Where(g => g.StudentId == id && g.Assignment.SectionId == section.SectionId && g.GradeDateTime > (DateTime)lastLogin).ToList();
                    var AssignmentList = context.Assignments.Where(a => a.SectionId == section.SectionId && a.AssignmentCreationDate != null && a.AssignmentCreationDate > lastLogin).ToList();

                    foreach (var grade in GradeList)
                    {
                        NotificationList.Add(new Notification(grade.Assignment.AssignmentTitle, 'G', section.Course.CourseCode + " " + section.Course.CourseNumber, $"/Courses/{section.SectionId}", grade.GradeDateTime));
                    }
                    foreach (var assignment in AssignmentList)
                    {
                        NotificationList.Add(new Notification(assignment.AssignmentTitle, 'A', section.Course.CourseCode + " " + section.Course.CourseNumber, $"/Courses/{section.SectionId}/Assignments/{assignment.AssignmentId}", (DateTime)assignment.AssignmentCreationDate));
                    }
                }

            }


        }
    }

    /// <summary>
    /// A data class for keeping track of notification details
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// The related data of what a notification is referring to, such as Assignment title that was created/graded
        /// </summary>
        public string NotificationTitle { get; set; }
        /// <summary>
        /// What type of notification it is, like 'G' for Grading or 'A' for assignment
        /// </summary>
        public char Type { get; set; } 
        public string CourseName { get; set; }
        public string Link { get; set; }
        public DateTime NotificationDate { get; set; }

        public Notification(string title, char type, string coursename, string link, DateTime notificationTime)
        {
            NotificationTitle = title;
            Type = type;
            CourseName = coursename;
            Link = link;
            NotificationDate = notificationTime;
        }
    }
}
