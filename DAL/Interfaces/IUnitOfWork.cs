using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Article> Articles { get; }
        IRepository<Comment> Comments { get; }
        IRepository<Category> Categories { get; }
        void Save();
    }

}
