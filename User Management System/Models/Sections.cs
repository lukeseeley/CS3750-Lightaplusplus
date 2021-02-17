using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.Models
{
    public class Sections
    {
        /// <summary>
        /// The Id for the section
        /// </summary>
        [Key]
        public int SectionId { get; set; }

        /// <summary>
        /// The FK ID for the course the section is based on
        /// </summary>
        [Required]
        public int CourseId { get; set; }

        /// <summary>
        /// The FK ID for the instructor of this section
        /// </summary>
        [Required]
        public int InstructorId { get; set; }

        /// <summary>
        /// The physical location of the class
        /// </summary>
        [Required, MaxLength(250)]
        public string SectionLocation { get; set; }

        /// <summary>
        /// The time of day this class stars
        /// </summary>
        [DataType(DataType.Date), Required]
        public DateTime SectionTimeStart { get; set; }

        [DataType(DataType.Date), Required]
        public DateTime SectionTimeEnd { get; set; }

        /// <summary>
        /// This stores a string for the days of the week it is taught for example: "MTW for Monday Tuesday and Wednesday"
        /// </summary>
        [Required, MaxLength(7)]
        public string DaysTaught { get; set; }

        /// <summary>
        /// The maximum capacity of students in a section
        /// </summary>
        [Required]
        public int SectionCapacity { get; set; }

        /// <summary>
        /// The Course this section is based on
        /// </summary>
        public virtual Courses Course { get; set; }

        /// <summary>
        /// The Instructor for this course
        /// </summary>
        public virtual Users Instructor { get; set; }
    }
}
