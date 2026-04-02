using BLL.DTO;
using PL.Menu;

namespace PL.Content.Render
{
    public static class ArticleUI
    {
        public static void RenderContent(ArticleDTO article, UserDTO? user)
        {
            MenuHelper.ShowHeader(article.Title, user?.Nickname);
            Console.WriteLine($"Автор: {article.AuthorName}\n" + new string('=', 40));
            Console.WriteLine($"{article.Content}\n" + new string('=', 40));

            if (user != null && article.AuthorName == user.Nickname)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[D] - ВИДАЛИТИ ЦЮ СТАТТЮ");
                Console.ResetColor();
            }
        }
    }
}
