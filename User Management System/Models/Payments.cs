using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.Models
{
    public class Payments
    {
        [Key]
        public int PaymentId { get; set; }
        [Required]
        public int UserId { get; set; }

        [Required]
        public int PaymentAmount { get; set; } // payment amount (double) * 100

        [Required, DataType(DataType.DateTime)]
        public DateTime PaymentDateTime { get; set; }

        public virtual Users User { get; set; }
    }
}
