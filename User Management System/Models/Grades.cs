using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.Models
{
    public class Grades
    {
        [Required]
        public int AssignmentId { get; set; }
        [Required]
        public int StudentId { get; set; }

        /// <summary>
        /// The date and time in which the assignment was graded by the instructor
        /// </summary>
        [Required, DataType(DataType.DateTime)]
        public DateTime GradeDateTime { get; set; }
        /// <summary>
        /// The numeric value given on the assignment (Must be between 0 and the assignments max points)
        /// </summary>
        [Required]
        public int GradeValue { get; set; }

        public virtual Assignments Assignment { get; set; }
        public virtual Users Student { get; set; }
    }
}
