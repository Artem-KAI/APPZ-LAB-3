using BLL.DTO;
using DAL.EF;
using DAL.Entities;

namespace BLL.Services
{
    public class BlogService : IDisposable
    {
        private readonly IUnitOfWork _db;

        public BlogService() => _db = new UnitOfWork();
        public IEnumerable<CommentDTO> GetCommentsByArticle(int articleId)
        {
            var comments = _db.Comments.GetAll().Where(c => c.ArticleId == articleId);

            return comments.Select(c => new CommentDTO
            {
                Id = c.Id,
                Text = c.Text,
                ParentId = c.ParentId, // ДОДАЙ ЦЕЙ РЯДОК!
                AuthorName = _db.Users.Get(c.AuthorId)?.Nickname ?? "Гість"
            }).ToList();
        }

        public UserDTO Register(string nick, string email, string pass, string confirm)
        {
            if (pass != confirm) throw new Exception("Паролі не збігаються!");

            // EF 8 підтримує AnyAsync, але для консолі залишаємо Any
            if (_db.Users.GetAll().Any(u => u.Email == email))
                throw new Exception("Користувач з таким Email вже існує!");

            var user = new User { Nickname = nick, Email = email, Password = pass };
            _db.Users.Create(user);
            _db.Save();

            return new UserDTO { Id = user.Id, Nickname = user.Nickname, Email = user.Email };
        }

        public UserDTO? Login(string email, string pass)
        {
            var user = _db.Users.GetAll()
                .FirstOrDefault(u => u.Email == email && u.Password == pass);

            return user == null ? null : new UserDTO
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Email = user.Email
            };
        }

        public IEnumerable<ArticleDTO> GetArticles()
        {
            var articlesFromDb = _db.Articles.GetAll();

            // ОСЬ ЦЕ І Є MAPPING (Відображення з DAL у BLL)
            return articlesFromDb.Select(a => new ArticleDTO
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                AuthorName = _db.Users.Get(a.AuthorId)?.Nickname ?? "Гість"
            }).ToList();
        }

        public void CreateComment(int userId, int articleId, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new Exception("Коментар не може бути порожнім!");

            var comment = new Comment
            {
                AuthorId = userId,
                ArticleId = articleId,
                Text = text
            };
            _db.Comments.Create(comment);
            _db.Save();
        }

        public void CreateCategory(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new Exception("Назва рубрики порожня!");
            _db.Categories.Create(new Category { Name = name });
            _db.Save();
        }

        public void CreateArticle(int userId, string title, string content)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new Exception("Заголовок порожній!");
            _db.Articles.Create(new Article
            {
                AuthorId = userId,
                Title = title,
                Content = content
            });
            _db.Save();
        }
        public void DeleteArticle(int userId, int articleId)
        {
            var article = _db.Articles.Get(articleId);

            if (article == null)
                throw new Exception("Статтю не знайдено.");

            // ПЕРЕВІРКА ПРАВ: Тільки автор може видалити
            if (article.AuthorId != userId)
                throw new Exception("Ви не маєте прав для видалення цієї статті!");

            _db.Articles.Delete(articleId);
            _db.Save();
        }

        public void DeleteComment(int userId, int commentId)
        {
            var comment = _db.Comments.Get(commentId);
            if (comment == null) throw new Exception("Коментар не знайдено.");

            if (comment.AuthorId != userId)
                throw new Exception("Ви можете видаляти тільки свої коментарі!");

            _db.Comments.Delete(commentId);
            _db.Save();
        }
        public IEnumerable<CategoryDTO> GetCategories()
        {
            return _db.Categories.GetAll().Select(c => new CategoryDTO { Id = c.Id, Name = c.Name });
        }
        public void ReplyToComment(int userId, int articleId, int parentId, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new Exception("Текст не може бути порожнім!");

            var parent = _db.Comments.Get(parentId);
            if (parent == null) throw new Exception("Коментар для відповіді не знайдено.");

            var reply = new Comment
            {
                AuthorId = userId,
                ArticleId = articleId,
                ParentId = parentId,
                Text = text
            };
            _db.Comments.Create(reply);
            _db.Save();
        }

        public void Dispose() => _db.Dispose();
    }

}