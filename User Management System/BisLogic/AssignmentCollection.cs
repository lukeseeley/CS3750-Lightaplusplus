using Lightaplusplus.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.BisLogic
{
    /// <summary>
    /// This is a class for gathering a list of assignments for a course paired with their grade values, if there are grades to acquire for a specific section.
    /// This class does not handle whether or not the user should be able to access a section
    /// </summary>
    public class AssignmentCollection
    {
        public List<AssignmentCollect> AssignmentGradeList { get; set; }
        
        public AssignmentCollection(ISession session, Lightaplusplus.Data.Lightaplusplus_SystemContext context, int sectionId)
        {
            var id = Session.getUserId(session);
            var usertype = Session.getUserType(session);

            AssignmentGradeList = new List<AssignmentCollect>();

            if (usertype == "I")
            {
                var assignments = context.Assignments.Where(a => a.SectionId == sectionId).ToList();
                foreach (var assignment in assignments)
                {
                    AssignmentGradeList.Add(new AssignmentCollect(assignment));
                }
            }
            else
            {
                var assignments = context.Assignments.Where(a => a.SectionId == sectionId).ToList();
                foreach (var assignment in assignments)
                {
                    var grade = context.Grades.FirstOrDefault(g => g.AssignmentId == assignment.AssignmentId && g.StudentId == id);
                    var submission = context.AssignmentSubmissions.FirstOrDefault(g => g.AssignmentId == assignment.AssignmentId && g.StudentId == id);
                    bool submitted = submission != null;
                    AssignmentGradeList.Add(new AssignmentCollect(assignment, grade, submitted));
                }
            }
        }

        public List<AssignmentCollect> getGradedAssignments()
        {
            var gradedAssignments = new List<AssignmentCollect>();
            foreach (var assignment in AssignmentGradeList)
            {
                if(assignment.Grade != null)
                {
                    gradedAssignments.Add(assignment);
                }
            }

            return gradedAssignments;
        }
    }

    /// <summary>
    /// A simple data class for bundling an Assignment and Grade object together
    /// </summary>
    public class AssignmentCollect
    {
        public Assignments Assignment { get; set; }
        public Grades Grade { get; set; }
        public bool Submitted { get; set; }

        public AssignmentCollect(Assignments assignment, Grades grade = null, bool submitted = false)
        {
            Assignment = assignment;
            Grade = grade;
            Submitted = submitted;
        }
    }
}
