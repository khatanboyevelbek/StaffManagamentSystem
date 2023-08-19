  using Microsoft.EntityFrameworkCore;
using StaffManagementSystem.Api.Domain.Entities;

namespace StaffManagementSystem.Api.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        { 
            Database.Migrate();
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Director> Directors { get; set; }
        public virtual DbSet<Kadr> Kadrs { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Vacation> Vacations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Id);

            modelBuilder.Entity<Director>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<Director>()
                .HasIndex(a => a.Id);

            modelBuilder.Entity<Kadr>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<Kadr>()
               .HasIndex(a => a.Id);

            modelBuilder.Entity<User>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<User>()
                .HasIndex(a => a.Id);

            modelBuilder.Entity<Vacation>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<Vacation>()
                .HasIndex(a => a.Id);
            modelBuilder.Entity<Vacation>()
                .HasIndex(a => a.UserId);
            modelBuilder.Entity<Vacation>()
                .HasOne(v => v.User)
                .WithMany(u => u.Vacations)
                .HasForeignKey(v => v.UserId);
        }
    }
}
