using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.Models
{
    public class Courses
    {
        public Courses()
        {
            this.Sections = new HashSet<Sections>();
        }
        
        /// <summary>
        /// The Id for the course
        /// </summary>
        [Key]
        public int CourseId { get; set; }

        /// <summary>
        /// The Code for the course; i.e. CS, ENGL, WEB
        /// </summary>
        [Required, MaxLength(10)]
        public string CourseCode { get; set; }

        /// <summary>
        /// The number for the course
        /// </summary>
        [Required]
        public int CourseNumber { get; set; }

        /// <summary>
        /// The name of the course
        /// </summary>
        [Required, MaxLength(50)]
        public string CourseName { get; set; }

        /// <summary>
        /// The description of the course
        /// </summary>
        [Required]
        public string CourseDescription { get; set; }

        /// <summary>
        /// The number of credit hours this course takes
        /// </summary>
        [Required]
        public int CourseCreditHours { get; set; }

        [Required, MaxLength(200)]
        public string CourseDepartment { get; set; }

        //FK links
        /// <summary>
        /// The sections made of this course
        /// </summary>
        public virtual ICollection<Sections> Sections { get; set; }

    }
}
