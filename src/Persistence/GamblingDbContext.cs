using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence
{
    public class GamblingDbContext : DbContext
    {
        public GamblingDbContext() { }

        public GamblingDbContext(DbContextOptions<GamblingDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(GamblingDbContext).Assembly);
        }

        public DbSet<ClientApplication> ClientApplications { get; set; }
        public DbSet<Account> Accounts { get; set; }

    }
}
