using Microsoft.EntityFrameworkCore;

namespace RESTful_Books_API.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Models.BookModel> Books { get; set; }
        public DbSet<Models.UserModel> Users { get; set; }
        public DbSet<Models.LoanModel> Loans { get; set; }

        public DbSet<Models.AdminModel> Admins { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Models.LoanModel>()
                .HasOne(l => l.User)
                .WithMany(u => u.Loans)
                .HasForeignKey(l => l.UserId);

            modelBuilder.Entity<Models.LoanModel>()
                .HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId);
        }
    }
}
