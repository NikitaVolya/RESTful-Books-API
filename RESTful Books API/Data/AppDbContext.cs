using Microsoft.EntityFrameworkCore;

namespace RESTful_Books_API.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Models.Book> Books { get; set; }
        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Loan> Loans { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Models.Loan>()
                .HasOne(l => l.User)
                .WithMany(u => u.Loans)
                .HasForeignKey(l => l.UserId);

            modelBuilder.Entity<Models.Loan>()
                .HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId);
        }
    }
}
