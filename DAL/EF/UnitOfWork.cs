using DAL.Repositories;
using DAL.Entities;

namespace DAL.EF
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Article> Articles { get; }
        IRepository<Comment> Comments { get; }
        IRepository<Category> Categories { get; }  
        void Save();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlogContext _db = new BlogContext();

        public UnitOfWork()
        {
            // Ця команда змушує СУБД перевірити наявність файлу та створити таблиці
            _db.Database.EnsureCreated();
        }

        public void Save() => _db.SaveChanges();
        public void Dispose() => _db.Dispose();

        // Інші репозиторії (як були раніше)
        public IRepository<User> Users => _userRepo ??= new EFRepository<User>(_db);
        public IRepository<Article> Articles => _articleRepo ??= new EFRepository<Article>(_db);
        public IRepository<Comment> Comments => _commentRepo ??= new EFRepository<Comment>(_db);
        public IRepository<Category> Categories => _catRepo ??= new EFRepository<Category>(_db);

        private EFRepository<User> _userRepo;
        private EFRepository<Article> _articleRepo;
        private EFRepository<Comment> _commentRepo;
        private EFRepository<Category> _catRepo;
    }
}