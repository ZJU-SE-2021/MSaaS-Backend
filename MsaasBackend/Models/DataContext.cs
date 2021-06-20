using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace MsaasBackend.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Hospital> Hospitals { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Physician> Physicians { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<MedicalRecord> MedicalRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable(nameof(Users))
                .HasData(new User
                {
                    Id = 1,
                    Username = "root",
                    PasswordHash = BC.EnhancedHashPassword("root"),
                    Role = "Admin"
                });
            modelBuilder.Entity<Hospital>().ToTable(nameof(Hospitals));
            modelBuilder.Entity<Department>().ToTable(nameof(Departments));
            modelBuilder.Entity<Physician>().ToTable(nameof(Physicians));
            modelBuilder.Entity<Appointment>().ToTable(nameof(Appointments));
            modelBuilder.Entity<MedicalRecord>().ToTable(nameof(MedicalRecords));
        }
    }
}