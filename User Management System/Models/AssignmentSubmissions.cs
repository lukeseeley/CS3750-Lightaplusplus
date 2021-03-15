using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.Models
{
    public class AssignmentSubmissions
    {
        [Key]
        public int SubmissionId { get; set; }
        [Required]
        public int AssignmentId { get; set; }
        [Required]
        public int StudentId { get; set; }

        /// <summary>
        /// The date and time in which the assignment was submitted
        /// </summary>
        [Required, DataType(DataType.DateTime)]
        public DateTime SubmissionDateTime { get; set; }

        /// <summary>
        /// The submission for the assignment.
        /// If the assignment is a text submission, this should be the raw text of the submission.
        /// If this is a file submission, then this should store a link to where the file is stored.
        /// </summary>
        [Required]
        public string Submission { get; set; }

        public virtual Assignments Assignment { get; set; }
        public virtual Users Student { get; set; }
    }
}
