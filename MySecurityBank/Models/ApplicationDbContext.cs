using Microsoft.EntityFrameworkCore;
using MySecurityBank.Models.MySecureBank.Models;
using System.Collections.Generic;

namespace MySecurityBank.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        //public DbSet<Customer> Customers => Set<Customer>();
    }
}
