using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.Models
{
    public class Users
    {
        public Users()
        {
            this.Links = new HashSet<UserLinks>();
            this.InstructorSections = new HashSet<Sections>();
            this.StudentSections = new HashSet<SectionStudents>();
        }

        /// <summary>
        /// The Id for the user
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The email of the user (Which is the user name, and should therefore be unique)
        /// </summary>
        [Required]
        public string email { get; set; }

        /// <summary>
        /// The users password, which is encrypted before being uploaded to the database
        /// </summary>
        [Required]
        public string password { get; set; }

        /// <summary>
        /// The date of birth of the user, user must be at least 18
        /// </summary>
        [DataType(DataType.Date), Required]
        public DateTime birthday { get; set; }

        /// <summary>
        /// The first name of the user
        /// </summary>
        [Required, MaxLength(50)]
        public string firstname { get; set; }

        /// <summary>
        /// The last name of the user
        /// </summary>
        [Required, MaxLength(50)]
        public string lastname { get; set; }

        /// <summary>
        /// The user type (Currently either S for student and I for instructor)
        /// </summary>
        [Required]
        public char usertype { get; set; }

        ////////////// Non Required data //////////////
        /// <summary>
        /// The phone number of the user
        /// </summary>
        [DataType(DataType.PhoneNumber), MaxLength(30)]
        public string phonenumber { get; set; }

        /// <summary>
        /// A biography of the user
        /// </summary>
        public string bio { get; set; }

        //Address
        /// <summary>
        /// The street address line 1
        /// </summary>
        [MaxLength(50)]
        public string addressline1 { get; set; }

        /// <summary>
        /// The street address line 2
        /// </summary>
        [MaxLength(50)]
        public string addressline2 { get; set; }

        /// <summary>
        /// The address city
        /// </summary>
        [MaxLength(50)]
        public string addresscity { get; set; }

        /// <summary>
        /// The address state
        /// </summary>
        [MaxLength(50)]
        public string addressstate { get; set; }

        /// <summary>
        /// The address zip code
        /// </summary>
        public int addresszip { get; set; }

        //FK links
        /// <summary>
        /// A collection of links for the given user
        /// </summary>
        public virtual ICollection<UserLinks> Links { get; set; }

        /// <summary>
        /// The collection of sections an instructor has
        /// </summary>
        public virtual ICollection<Sections> InstructorSections { get; set; }

        /// <summary>
        /// This is the collection of sections a student is enrolled in
        /// </summary>
        public virtual ICollection<SectionStudents> StudentSections { get; set; }

        /// <summary>
        /// The profile picture for the users
        /// </summary>
        public virtual UserPictures Picture { get; set; }

    }
}
