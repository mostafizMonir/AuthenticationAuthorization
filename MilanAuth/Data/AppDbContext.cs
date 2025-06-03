using Microsoft.EntityFrameworkCore;

namespace MilanAuth.Data;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<ImageProcessingJob> ImageJobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImageProcessingJob>(b =>
            {
                b.HasKey(x => x.JobId);
                b.Property(x => x.OriginalImagePath).IsRequired();
            });
        }
}
