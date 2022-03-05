using Microsoft.EntityFrameworkCore;

namespace CollectorApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
        {
        }

        public DbSet<Login>? Login { get; set; }
        public DbSet<Link>? Link { get; set; }
        public DbSet<Collected>? Collected { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
