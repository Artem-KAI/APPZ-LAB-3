using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DAL.EF
{
    // Цей клас використовується ТІЛЬКИ утилітами EF Core (при Add-Migration)
    public class BlogContextFactory : IDesignTimeDbContextFactory<BlogContext>
    {
        public BlogContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BlogContext>();

            // Вказуємо рядок підключення спеціально для генерації міграцій
            optionsBuilder.UseSqlite("Data Source=blog_system.db");

            return new BlogContext(optionsBuilder.Options);
        }
    }
}