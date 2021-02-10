using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Lightaplusplus.Models;

namespace Lightaplusplus.Data
{
    public class Lightaplusplus_SystemContext : DbContext
    {
        public Lightaplusplus_SystemContext (DbContextOptions<Lightaplusplus_SystemContext> options)
            : base(options)
        {
        }

        public DbSet<Lightaplusplus.Models.Users> Users { get; set; }
    }
}
