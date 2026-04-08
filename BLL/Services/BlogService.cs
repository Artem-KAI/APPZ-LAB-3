using BLL.DTO;
using DAL.EF;
using DAL.Entities;

namespace BLL.Services
{
    public class BlogService : IDisposable
    {
        private readonly IUnitOfWork _db;

        public BlogService() => _db = new UnitOfWork();

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

//using BLL.DTO;
//using DAL.EF;
//using DAL.Entities;

//namespace BLL.Services
//{
//    public class BlogService : IDisposable
//    {
//        private readonly IUnitOfWork _db;

//        public BlogService() => _db = new UnitOfWork();
//        public IEnumerable<CommentDTO> GetCommentsByArticle(int articleId)
//        {
//            var comments = _db.Comments.GetAll().Where(c => c.ArticleId == articleId);

//            return comments.Select(c => new CommentDTO
//            {
//                Id = c.Id,
//                Text = c.Text,
//                ParentId = c.ParentId, 
//                AuthorName = _db.Users.Get(c.AuthorId)?.Nickname ?? "Гість"
//            }).ToList();
//        }

//        public UserDTO Register(string nick, string email, string pass, string confirm)
//        {
//            if (pass != confirm)
//            {
//                throw new Exception("Паролі не збігаються!");
//            }

//            if (_db.Users.GetAll().Any(u => u.Email == email))
//            {
//                throw new Exception("Користувач з таким Email вже існує!");
//            }

//            var user = new User { 
//                Nickname = nick, 
//                Email = email, 
//                Password = pass 
//            };

//            _db.Users.Create(user);
//            _db.Save();

//            return new UserDTO { 
//                Id = user.Id, 
//                Nickname = 
//                user.Nickname, 
//                Email = user.Email 
//            };
//        }

//        public UserDTO? Login(string email, string pass)
//        {
//            var user = _db.Users.GetAll()
//                .FirstOrDefault(u => u.Email == email && u.Password == pass);

//            return user == null ? null : new UserDTO
//            {
//                Id = user.Id,
//                Nickname = user.Nickname,
//                Email = user.Email
//            };
//        }

//        public IEnumerable<ArticleDTO> GetArticles()
//        {
//            var articlesFromDb = _db.Articles.GetAll();

//            return articlesFromDb.Select(a => new ArticleDTO
//            {
//                Id = a.Id,
//                Title = a.Title,
//                Content = a.Content,
//                CategoryId = a.CategoryId,
//                AuthorName = _db.Users.Get(a.AuthorId)?.Nickname ?? "Гість"
//            }).ToList();
//        }

//        public void CreateComment(int userId, int articleId, string text)
//        {
//            if (string.IsNullOrWhiteSpace(text))
//            {
//                throw new Exception("Коментар не може бути порожнім!");
//            }

//            var comment = new Comment
//            {
//                AuthorId = userId,
//                ArticleId = articleId,
//                Text = text
//            };
//            _db.Comments.Create(comment);
//            _db.Save();
//        }

//        public void CreateCategory(string name)
//        {
//            if (string.IsNullOrWhiteSpace(name))
//            {
//                throw new Exception("Назва рубрики порожня!");
//            }

//            _db.Categories.Create(new Category { Name = name });
//            _db.Save();
//        }

//        public void CreateArticle(int userId, int categoryId, string title, string content)
//        {
//            if (string.IsNullOrWhiteSpace(title)) throw new Exception("Заголовок порожній!");
//            _db.Articles.Create(new Article
//            {
//                AuthorId = userId,
//                CategoryId = categoryId,
//                Title = title,
//                Content = content
//            });
//            _db.Save();
//        }
//        public void DeleteArticle(int userId, int articleId)
//        {
//            var article = _db.Articles.Get(articleId);

//            if (article == null)
//                throw new Exception("Статтю не знайдено.");

//            if (article.AuthorId != userId)
//                throw new Exception("Ви не маєте прав для видалення цієї статті!");

//            _db.Articles.Delete(articleId);
//            _db.Save();
//        }

//        public void DeleteComment(int userId, int commentId)
//        {
//            var comment = _db.Comments.Get(commentId);
//            if (comment == null) 
//                throw new Exception("Коментар не знайдено.");

//            if (comment.AuthorId != userId)
//                throw new Exception("Ви можете видаляти тільки свої коментарі!");

//            var replies = _db.Comments.GetAll().Where(c => c.ParentId == commentId).ToList();

//            foreach (var reply in replies)
//            {
//                _db.Comments.Delete(reply.Id);
//            }

//            _db.Comments.Delete(commentId);
//            _db.Save();
//        }
//        public IEnumerable<CategoryDTO> GetCategories()
//        {
//            return _db.Categories.GetAll().Select(c => new CategoryDTO { Id = c.Id, Name = c.Name });
//        }

//        public void ReplyToComment(int userId, int articleId, int parentId, string text)
//        {
//            if (string.IsNullOrWhiteSpace(text)) throw new Exception("Текст не може бути порожнім!");

//            var parent = _db.Comments.Get(parentId);

//            if (parent == null) 
//                throw new Exception("Коментар для відповіді не знайдено.");

//            var reply = new Comment
//            {
//                AuthorId = userId,
//                ArticleId = articleId,
//                ParentId = parentId,
//                Text = text
//            };
//            _db.Comments.Create(reply);
//            _db.Save();
//        }
//        public void Dispose() => _db.Dispose();
//    }
//}