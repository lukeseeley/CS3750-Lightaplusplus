using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.Models
{
    public class CreditCard
    {
        /// <summary>
        /// Credit card number (4242424242424242)
        /// </summary>
        public string cardNumber { get; set; }
        /// <summary>
        /// Credit card cvc (111, the 3 numbers on the back)
        /// </summary>
        public string cvc { get; set; }
        /// <summary>
        /// Credit card expiration month
        /// </summary>
        public string exp_month { get; set; }
        /// <summary>
        /// Credit card expiration year (2022)
        /// </summary>
        public string exp_year { get; set; }
    }
}
