using Microsoft.EntityFrameworkCore;
using MySecurityBank.Models.MySecureBank.Models;
using System.Collections.Generic;

namespace MySecurityBank.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Customer> Customers => Set<Customer>();
    }
}
