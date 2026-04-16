using BLL.DTO;
using DAL.Interfaces;
using DAL.Entities;

namespace BLL.Services
{
    public class BlogService : IDisposable
    {
        private readonly IUnitOfWork _db;

        public BlogService(IUnitOfWork db)
        {
            _db = db;
        }


        // Statements
        public IEnumerable<ArticleDTO> GetArticles()
        {
            var articlesFromDb = _db.Articles.GetAll();

            List<ArticleDTO> resultList = new List<ArticleDTO>();

            foreach (var item in articlesFromDb)
            {
                ArticleDTO dto = MapToArticleDTO(item);
                
                resultList.Add(dto);
            }
             
            return resultList;
        }

        public void CreateArticle(int userId, int categoryId, string title, string content)
        {
            ValidateString(title, "Заголовок");

            _db.Articles.Create(new Article 
            { AuthorId = userId, 
                CategoryId = categoryId, 
                Title = title, 
                Content = content 
            });

            _db.Save();
        }

        public void EditArticle(int userId, int articleId, string newTitle, string newContent)
        {
            var article = _db.Articles.Get(articleId);

            if (article == null)
                throw new Exception("Статтю не знайдено.");

            if (article.AuthorId != userId)
                throw new Exception("Ви не маєте прав на редагування цієї статті!");

            if (!string.IsNullOrWhiteSpace(newTitle))
            {
                article.Title = newTitle;
            }

            if (!string.IsNullOrWhiteSpace(newContent))
            {
                article.Content = newContent;
            }

            _db.Articles.Update(article);
            _db.Save();
        }

        public void DeleteArticle(int userId, int articleId)
        {
            var article = _db.Articles.Get(articleId);

            if (article == null) 
                throw new Exception("Статтю не знайдено.");
            if (article.AuthorId != userId) 
                throw new Exception("Ви не маєте прав на видалення!");

            _db.Articles.Delete(articleId);
            _db.Save();
        }


        // Comments
        public IEnumerable<CommentDTO> GetCommentsByArticle(int articleId)
        { 
            var allCommentsFromDb = _db.Comments.GetAll();
             
            List<CommentDTO> filteredComments = new List<CommentDTO>();
             
            foreach (var c in allCommentsFromDb)
            { 
                if (c.ArticleId == articleId)
                { 
                    CommentDTO dto = MapToCommentDTO(c);
                     
                    filteredComments.Add(dto);
                }
            }
             
            return filteredComments;
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
            var comment = _db.Comments.Get(commentId);
             
            if (comment == null)
            {
                throw new Exception("Коментар не знайдено.");
            }
             
            if (comment.AuthorId != userId)
            {
                throw new Exception("Це не ваш коментар! Ви не маєте прав на його видалення.");
            }
             
            var allComments = _db.Comments.GetAll();
            List<Comment> repliesToDelete = new List<Comment>();

            foreach (var c in allComments)
            {
                if (c.ParentId == commentId)
                {
                    repliesToDelete.Add(c);
                }
            }
             
            foreach (var r in repliesToDelete)
            {
                _db.Comments.Delete(r.Id);
            }
             
            _db.Comments.Delete(commentId);
             
            _db.Save();
        }


        // Categories
        public IEnumerable<CategoryDTO> GetCategories()
        {
            var categoriesFromDb = _db.Categories.GetAll();

            List<CategoryDTO> resultList = new List<CategoryDTO>();

            foreach (var c in categoriesFromDb)
            {
                CategoryDTO dto = new CategoryDTO();
                dto.Id = c.Id;       
                dto.Name = c.Name;    

                resultList.Add(dto);
            }

            return resultList;
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
            var category = _db.Categories.Get(categoryId);
            if (category == null)
                throw new Exception("Категорію не знайдено.");
          
            var allArticles = _db.Articles.GetAll();
            bool found = false;

            foreach (var a in allArticles)
            {
                if (a.CategoryId == categoryId)
                {
                    found = true;
                    break; 
                }
            }

            if (found)
            {
                throw new Exception("У цій категорії ще є статті!");
            }

            _db.Categories.Delete(categoryId);
            _db.Save();
        }


        // Helpers
        private void ValidateString(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value)) 
                throw new Exception($"{fieldName} не може бути порожнім!");
        }

        private ArticleDTO MapToArticleDTO(Article a)
        {
            var author = _db.Users.Get(a.AuthorId);

            string name;

            if (author != null)
                name = author.Nickname;

            else
                name = "Гість";

            return new ArticleDTO 
            { 
                Id = a.Id, 
                Title = a.Title, 
                Content = a.Content, 
                CategoryId = a.CategoryId, 
                AuthorName = name
            };
        }

        private CommentDTO MapToCommentDTO(Comment c)
        {
            var author = _db.Users.Get(c.AuthorId);

            string name;

            if (author != null)
                name = author.Nickname;
            
            else          
                name = "Гість";


            return new CommentDTO
            {
                Id = c.Id,
                Text = c.Text,
                ParentId = c.ParentId,
                AuthorName = name
            };
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
