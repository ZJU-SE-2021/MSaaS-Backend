using Microsoft.EntityFrameworkCore;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable(nameof(Users));
            modelBuilder.Entity<Hospital>().ToTable(nameof(Hospitals));
            modelBuilder.Entity<Department>().ToTable(nameof(Departments));
        }
    }
}