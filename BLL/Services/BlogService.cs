using BLL.DTO;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _db;

        public BlogService(IUnitOfWork db)
        {
            _db = db;
        }

        // -- Articles --

        public IEnumerable<ArticleDTO> GetArticles()
        {
            return _db.Articles.GetAll().Select(MapToArticleDTO).ToList();
        }

        public void CreateArticle(int userId, int categoryId, string title, string content)
        {
            ValidateString(title, "Заголовок");

            _db.Articles.Create(new Article
            {
                AuthorId = userId,
                CategoryId = categoryId,
                Title = title,
                Content = content
            });

            _db.Save();
        }

        public void EditArticle(int userId, int articleId, string newTitle, string newContent)
        {
            var article = _db.Articles.Get(articleId)
                ?? throw new KeyNotFoundException("Статтю не знайдено.");

            if (article.AuthorId != userId)
                throw new UnauthorizedAccessException("Ви не маєте прав на редагування цієї статті!");

            if (!string.IsNullOrWhiteSpace(newTitle))
                article.Title = newTitle;

            if (!string.IsNullOrWhiteSpace(newContent))
                article.Content = newContent;

            _db.Articles.Update(article);
            _db.Save();
        }

        public void DeleteArticle(int userId, int articleId)
        {
            var article = _db.Articles.Get(articleId)
                ?? throw new KeyNotFoundException("Статтю не знайдено.");

            if (article.AuthorId != userId)
                throw new UnauthorizedAccessException("Ви не маєте прав на видалення!");

            _db.Articles.Delete(articleId);
            _db.Save();
        }

        // -- Comments --

        public IEnumerable<CommentDTO> GetCommentsByArticle(int articleId)
        {
            return _db.Comments.GetAll()
                .Where(c => c.ArticleId == articleId)
                .Select(MapToCommentDTO)
                .ToList();
        }

        public void CreateComment(int userId, int articleId, string text)
        {
            ValidateString(text, "Коментар");

            _db.Comments.Create(new Comment
            {
                AuthorId = userId,
                ArticleId = articleId,
                Text = text
            });

            _db.Save();
        }

        public void ReplyToComment(int userId, int articleId, int parentId, string text)
        {
            ValidateString(text, "Відповідь");

            _db.Comments.Create(new Comment
            {
                AuthorId = userId,
                ArticleId = articleId,
                ParentId = parentId,
                Text = text
            });

            _db.Save();
        }

        public void DeleteComment(int userId, int commentId)
        {
            var comment = _db.Comments.Get(commentId)
                ?? throw new KeyNotFoundException("Коментар не знайдено.");

            if (comment.AuthorId != userId)
                throw new UnauthorizedAccessException("Це не ваш коментар! Ви не маєте прав на його видалення.");

            var repliesToDelete = _db.Comments.GetAll().Where(c => c.ParentId == commentId).ToList();
            foreach (var reply in repliesToDelete)
            {
                _db.Comments.Delete(reply.Id);
            }

            _db.Comments.Delete(commentId);
            _db.Save();
        }

        // -- Categories --

        public IEnumerable<CategoryDTO> GetCategories()
        {
            return _db.Categories.GetAll().Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
        }

        public void CreateCategory(string name)
        {
            ValidateString(name, "Назва рубрики");

            _db.Categories.Create(new Category
            {
                Name = name
            });

            _db.Save();
        }

        public void DeleteCategory(int categoryId)
        {
            var category = _db.Categories.Get(categoryId)
                ?? throw new KeyNotFoundException("Категорію не знайдено.");

            if (_db.Articles.GetAll().Any(a => a.CategoryId == categoryId))
                throw new InvalidOperationException("У цій категорії ще є статті!");

            _db.Categories.Delete(categoryId);
            _db.Save();
        }

        // -- Helpers --

        private static void ValidateString(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{fieldName} не може бути порожнім!");
        }

        private ArticleDTO MapToArticleDTO(Article article)
        {
            var author = _db.Users.Get(article.AuthorId);

            return new ArticleDTO
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                CategoryId = article.CategoryId,
                AuthorName = author?.Nickname ?? "Гість"
            };
        }

        private CommentDTO MapToCommentDTO(Comment comment)
        {
            var author = _db.Users.Get(comment.AuthorId);

            return new CommentDTO
            {
                Id = comment.Id,
                Text = comment.Text,
                ParentId = comment.ParentId,
                AuthorName = author?.Nickname ?? "Гість"
            };
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
} 