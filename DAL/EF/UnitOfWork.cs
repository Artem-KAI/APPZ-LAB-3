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

        private EFRepository<User> _userRepo;
        private EFRepository<Article> _articleRepo;
        private EFRepository<Comment> _commentRepo;
        private EFRepository<Category> _catRepo;

        public UnitOfWork()
        {
            _db.Database.EnsureCreated();
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }


        public IRepository<User> Users
        {
            get
            {
                if (_userRepo == null)
                {
                    _userRepo = new EFRepository<User>(_db);
                }

                return _userRepo;
            }
        }
        public IRepository<Article> Articles
        {
            get
            {
                if (_articleRepo == null)
                {
                    _articleRepo = new EFRepository<Article>(_db);
                }
                return _articleRepo;
            }
        }
        public IRepository<Comment> Comments
        {
            get
            {
                if (_commentRepo == null)
                {
                    _commentRepo = new EFRepository<Comment>(_db);
                }
                return _commentRepo;
            }
        }
        public IRepository<Category> Categories
        {
            get
            {
                if (_catRepo == null)
                {
                    _catRepo = new EFRepository<Category>(_db);
                }
                return _catRepo;
            }
        }

        //public IRepository<User> Users => _userRepo ??= new EFRepository<User>(_db);
        //public IRepository<Article> Articles => _articleRepo ??= new EFRepository<Article>(_db);
        //public IRepository<Comment> Comments => _commentRepo ??= new EFRepository<Comment>(_db);
        //public IRepository<Category> Categories => _catRepo ??= new EFRepository<Category>(_db);
    }
}