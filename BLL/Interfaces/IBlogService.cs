using BLL.DTO;

namespace BLL.Interfaces
{
    public interface IBlogService : IDisposable
    {
        // Articles
        IEnumerable<ArticleDTO> GetArticles();
        void CreateArticle(int userId, int categoryId, string title, string content);
        void EditArticle(int userId, int articleId, string newTitle, string newContent);
        void DeleteArticle(int userId, int articleId);

        // Comments
        IEnumerable<CommentDTO> GetCommentsByArticle(int articleId);
        void CreateComment(int userId, int articleId, string text);
        void ReplyToComment(int userId, int articleId, int parentId, string text);
        void DeleteComment(int userId, int commentId);

        // Categories
        IEnumerable<CategoryDTO> GetCategories();
        void CreateCategory(string name);
        void DeleteCategory(int categoryId);
    }
}
