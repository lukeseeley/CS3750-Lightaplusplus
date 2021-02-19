using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.Models
{
    public class SectionStudents
    {
        /// <summary>
        /// The Id for the related section
        /// </summary>
        [Required]
        public int SectionId { get; set; }

        [Required]
        public int StudentId { get; set; }

        /// <summary>
        /// The associated section
        /// </summary>
        public virtual Sections Section { get; set; }

        /// <summary>
        /// The student that has registered for the section
        /// </summary>
        public virtual Users Student { get; set; }
    }
}
