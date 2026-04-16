using DAL.Repositories;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlogContext _db;
       
        private EFRepository<User> _userRepo;
        private EFRepository<Article> _articleRepo;
        private EFRepository<Comment> _commentRepo;
        private EFRepository<Category> _catRepo;

        public UnitOfWork(BlogContext context)
        {
            _db = context;
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
    }
}