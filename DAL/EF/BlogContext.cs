using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace DAL.EF
{
    public class BlogContext : DbContext
    {
        // Використовуємо ініціалізацію за замовчуванням для EF 8
        public DbSet<User> Users => Set<User>();
        public DbSet<Article> Articles => Set<Article>();
        public DbSet<Comment> Comments => Set<Comment>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // СУБД SQLite створить файл бази в папці з програмою
            optionsBuilder.UseSqlite("Data Source=blog_system.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Налаштування зв'язків для СУБД
            modelBuilder.Entity<Article>()
                .HasOne(a => a.Author)
                .WithMany()
                .HasForeignKey(a => a.AuthorId);
        }
    }
}