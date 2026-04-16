using DAL.EF;
using DAL.Interfaces;
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

        public IEnumerable<T> GetAll()
        {
            return _dbSet.AsNoTracking().ToList();
        }

        public T Get(int id)
        {
            var item = _dbSet.Find(id);
            return item!;
        }

        public void Create(T item)
        {
            _dbSet.Add(item);
        }
        public void Update(T item)
        {
            _dbSet.Update(item);
        }

        public void Delete(int id)
        {
            var item = _dbSet.Find(id);

            if (item == null)
            {
                return;
            }

            _dbSet.Remove(item);
        }
    }
}