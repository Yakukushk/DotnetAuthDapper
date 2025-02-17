using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data
{
    public class DataContextEF : DbContext
    {
        private readonly IConfiguration _config;

        public DataContextEF(DbContextOptions<DataContextEF> options, IConfiguration config) : base(options)
        {
            _config = config;
        }

        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UserJobInfo> UserJobInfos { get; set; }
        public virtual DbSet<UserSalary> UserSalary { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"),
                    optionsBuilder => optionsBuilder.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("TutorialAppSchema");

            modelBuilder.Entity<Users>()
                .ToTable("Users")
                .HasKey(e => e.UserId);

            modelBuilder.Entity<UserSalary>()
                .ToTable("UserSalary")
                .HasKey(e => e.UserId);

            modelBuilder.Entity<UserJobInfo>()
                .ToTable("UserJobInfo")
                .HasKey(e => e.UserId);
        }
    }
}