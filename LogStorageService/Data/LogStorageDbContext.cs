using LogStorageService.Models;
using Microsoft.EntityFrameworkCore;

namespace LogStorageService.Data
{
    public class LogStorageDbContext : DbContext
    {
        public LogStorageDbContext(DbContextOptions<LogStorageDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogModel>(entity =>
            {
                entity.ToTable("Logs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IsChecked).IsRequired();

            });
        }

        public DbSet<LogModel> Logs { get; set; }
    }

}
