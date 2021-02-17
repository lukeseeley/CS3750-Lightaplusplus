using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.Models
{
    public class UserPicture
    {
        /// <summary>
        /// The Id for the specific picture entry
        /// </summary>
        [Key]
        public int PictureID { get; set; }

        /// <summary>
        /// The related User to this link
        /// </summary>
        [Required]
        public int UserID { get; set; }

        /// <summary>
        /// The actual pic that is being stored
        /// </summary>
        public byte[] profilepic { get; set; }

        /// <summary>
        /// The User this link belongs to
        /// </summary>
        public virtual Users User { get; set; }
    }
}
