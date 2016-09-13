using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GovernmentInformation.Models
{
    public class GovernmentInformationDbContext : DbContext
  {
    public DbSet<Query> Queries { get; set; }

    public DbSet<User> Users { get; set; }

    public GovernmentInformationDbContext(DbContextOptions<GovernmentInformationDbContext> options)
            : base(options)
    {
    }

    public GovernmentInformationDbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
    }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=GovernmentInformation;integrated security=True");
        }
    }
}
