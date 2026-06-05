using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BloodBankManagementSystem.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Donor> Donors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<BloodGroup> BloodGroups { get; set; }
        public DbSet<DonationRecord> DonationRecords { get; set; }
        public DbSet<BloodRequest> BloodRequests { get; set; }
        public DbSet<BloodDistribution> BloodDistributions { get; set; }
        public DbSet<DonationCamp> DonationCamps { get; set; }
        public DbSet<CampRegistration> CampRegistrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Blood Groups with static date
            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<BloodGroup>().HasData(
                new BloodGroup { BloodGroupId = 1, BloodType = "A+", AvailableUnits = 10, LastUpdated = seedDate },
                new BloodGroup { BloodGroupId = 2, BloodType = "A-", AvailableUnits = 5, LastUpdated = seedDate },
                new BloodGroup { BloodGroupId = 3, BloodType = "B+", AvailableUnits = 8, LastUpdated = seedDate },
                new BloodGroup { BloodGroupId = 4, BloodType = "B-", AvailableUnits = 3, LastUpdated = seedDate },
                new BloodGroup { BloodGroupId = 5, BloodType = "O+", AvailableUnits = 15, LastUpdated = seedDate },
                new BloodGroup { BloodGroupId = 6, BloodType = "O-", AvailableUnits = 7, LastUpdated = seedDate },
                new BloodGroup { BloodGroupId = 7, BloodType = "AB+", AvailableUnits = 6, LastUpdated = seedDate },
                new BloodGroup { BloodGroupId = 8, BloodType = "AB-", AvailableUnits = 2, LastUpdated = seedDate }
            );
        }
    }
}
