using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace User_Management_System.Models
{
    public class Users
    {
        public int ID { get; set; }
        public string email { get; set; }
        public string password { get; set; }

        [DataType(DataType.Date)]
        public DateTime birthday { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }
}
