using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// import the Entities (database models representing structure of tables in database)
using SustiVest.Data.Entities;

namespace SustiVest.Data.Repositories
{
    // The Context is How EntityFramework communicates with the database
    // We define DbSet properties for each table in the database
    public class DatabaseContext : DbContext
    {
        // authentication store
        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ForgotPassword> ForgotPasswords { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        // Configure the context with logging - remove in production
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // remove in production 
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging();
        }

        public static DbContextOptionsBuilder<DatabaseContext> OptionsBuilder => new();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().ToTable("Company");
            modelBuilder.Entity<Company>()
           .HasKey(c => c.CR_No);
            modelBuilder.Entity<User>().HasKey(u => u.Id);   
        }

        // Convenience method to recreate the database thus ensuring the new database takes 
        // account of any changes to Models or DatabaseContext. ONLY to be used in development
        // public void Initialise()
        // {
        //     Database.EnsureDeleted();
        //     Database.EnsureCreated();
        // }

    }
}
