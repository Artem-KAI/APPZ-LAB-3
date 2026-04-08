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
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // СУБД SQLite створить файл бази в папці з програмою
            optionsBuilder.UseSqlite("Data Source=blog_system.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Твій існуючий зв'язок з автором
            modelBuilder.Entity<Article>()
                .HasOne(a => a.Author)
                .WithMany()
                .HasForeignKey(a => a.AuthorId);

            // НОВЕ: Налаштування зв'язку Стаття -> Рубрика
            modelBuilder.Entity<Article>()
                .HasOne(a => a.Category)
                .WithMany() // У однієї рубрики багато статей
                .HasForeignKey(a => a.CategoryId);
        }
    }
}