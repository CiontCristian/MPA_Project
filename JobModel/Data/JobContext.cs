using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JobModel.Models;

namespace JobModel.Data
{
    public class JobContext : DbContext
    {
        public JobContext(DbContextOptions<JobContext> options) : base(options)
        {}

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<CompanyOffer> CompanyOffers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>().ToTable("Job");
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Company>().ToTable("Company");
            modelBuilder.Entity<Application>().ToTable("Application");
            modelBuilder.Entity<Application>().HasKey(a => new { a.JobID, a.ClientID });
            modelBuilder.Entity<CompanyOffer>().HasKey(a => new { a.CompanyID, a.JobID });
        }
    }
}
