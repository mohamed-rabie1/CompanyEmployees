using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repository.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryContext:IdentityDbContext<User>
    {
        public RepositoryContext(DbContextOptions options) :base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            //modelBuilder.ApplyConfiguration(new CompanyConfiguration()());
        }
        public DbSet<Company> Companies { get; set; } //EF Core is going to use for the communication with the database
        public DbSet<Employee> Employees { get; set; }
    }
}
