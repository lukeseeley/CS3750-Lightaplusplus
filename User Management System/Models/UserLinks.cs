using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.Models
{
    public class UserLinks
    {
        /// <summary>
        /// The Id for the specific link entry
        /// </summary>
        public int LinkId { get; set; }

        /// <summary>
        /// The related User to this link
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// The actual URL link that is being stored
        /// </summary>
        [DataType(DataType.Url), Required, MaxLength(250)]
        public string link { get; set; }

        /// <summary>
        /// The User this link belongs to
        /// </summary>
        public virtual Users User { get; set; }
    }
}
