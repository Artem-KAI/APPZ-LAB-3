using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace DAL.EF
{
    public class BlogContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Article> Articles => Set<Article>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Category> Categories => Set<Category>();

        public BlogContext(DbContextOptions<BlogContext> options) : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            modelBuilder.Entity<Article>()
                .HasOne(a => a.Author)
                .WithMany()
                .HasForeignKey(a => a.AuthorId);
             
            modelBuilder.Entity<Article>()
                .HasOne(a => a.Category)
                .WithMany() 
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}