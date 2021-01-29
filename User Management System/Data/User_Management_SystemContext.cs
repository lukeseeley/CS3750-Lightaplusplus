using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using User_Management_System.Models;

namespace User_Management_System.Data
{
    public class User_Management_SystemContext : DbContext
    {
        public User_Management_SystemContext (DbContextOptions<User_Management_SystemContext> options)
            : base(options)
        {
        }

        public DbSet<User_Management_System.Models.Users> Users { get; set; }
    }
}
