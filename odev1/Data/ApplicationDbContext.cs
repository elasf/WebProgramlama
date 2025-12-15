using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using odev1.Models;

namespace odev1.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserDetails>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
           
                        

        }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Expertise> Expertises { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<TrainerExpertise> TrainerExpertises { get; set; }
        public DbSet<TrainerService> TrainerServices { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            // Trainer <-> Expertise (Many-to-Many)
            modelBuilder.Entity<TrainerExpertise>()
                .HasKey(te => new { te.trainerId, te.expertiseId });

            modelBuilder.Entity<TrainerExpertise>()
                .HasOne(te => te.trainer)
                .WithMany(t => t.trainerExpertises)
                .HasForeignKey(te => te.trainerId);

            modelBuilder.Entity<TrainerExpertise>()
                .HasOne(te => te.expertise)
                .WithMany(e => e.trainerExpertise)
                .HasForeignKey(te => te.expertiseId);


            // Trainer <-> Service (Many-to-Many)
            modelBuilder.Entity<TrainerService>()
                .HasKey(ts => new { ts.trainerId, ts.serviceId });

            modelBuilder.Entity<TrainerService>()
                .HasOne(ts => ts.trainer)
                .WithMany(t => t.trainerServices)
                .HasForeignKey(ts => ts.trainerId);

            modelBuilder.Entity<TrainerService>()
                .HasOne(ts => ts.service)
                .WithMany(s => s.trainerService)
                .HasForeignKey(ts => ts.serviceId);
        }
    }
}
