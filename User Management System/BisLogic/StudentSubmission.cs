using Lightaplusplus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.BisLogic
{
    public class StudentSubmission
    {
        public AssignmentSubmissions AssignmentSubmission { get; set; }

        public Grades Grade { get; set; }

        public StudentSubmission(AssignmentSubmissions submission, Grades grade)
        {
            AssignmentSubmission = submission;
            Grade = grade;
        }
    }
    // submission page
    //https://localhost:44300/Courses/1011/Grades/2015?id=13
    // grade page
    //https://localhost:44300/Courses/1011/Grades/2015/34?id=13
}
