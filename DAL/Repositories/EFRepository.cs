using DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class EFRepository<T> : IRepository<T> where T : class
    {
        private readonly BlogContext _db;
        private readonly DbSet<T> _dbSet;

        public EFRepository(BlogContext context)
        {
            _db = context;
            _dbSet = context.Set<T>();
        }

        public IEnumerable<T> GetAll() => _dbSet.AsNoTracking().ToList();

        public T Get(int id) => _dbSet.Find(id)!;

        public void Create(T item) => _dbSet.Add(item);

        public void Delete(int id)
        {
            T item = _db.Set<T>().Find(id);
            if (item != null)
            {
                _db.Set<T>().Remove(item);
            }
        }
    }
}