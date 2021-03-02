using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.Models
{
    public class Assignments
    {
        [Key]
        public int AssignmentId { get; set; }

        [Required]
        public int SectionId { get; set; }

        /// <summary>
        /// The title of the description
        /// </summary>
        [Required, MaxLength(50)]
        public string AssignmentTitle { get; set; }

        /// <summary>
        /// The description of the assignment
        /// </summary>
        [Required]
        public string AssignmentDescription { get; set; }

        /// <summary>
        /// The Due date and time for the assignment
        /// </summary>
        [Required, DataType(DataType.DateTime)]
        public DateTime AssignmentDueDateTime { get; set; }

        /// <summary>
        /// The maximum number of points that can be awarded for this assignment
        /// </summary>
        [Required]
        public int? AssignmentMaxPoints { get; set; }

        /// <summary>
        /// The submission type allowed for this assignment
        /// Types include: F -> File submission; T -> Text Submission
        /// </summary>
        [Required]
        public char AssignmentSubmissionType { get; set; }

        public virtual Sections Section { get; set; }

    }
}
