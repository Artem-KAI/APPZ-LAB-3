using BLL.DTO;
using BLL.Services;
using PL.Content.Handler;
using PL.Content.Render;
using PL.Menu;

namespace PL.Content
{
    public static class ContentManager
    {
        public static void ShowArticlesMenu(BlogService service, UserDTO user)
        {
            while (true)
            {
                MenuHelper.ShowHeader("Список статей", user?.Nickname);
                var articles = service.GetArticles().ToList();

                if (!articles.Any())
                {
                    Console.WriteLine("На жаль, статтей поки немає.");
                }
                else
                {
                    Console.WriteLine("{0,-5} | {1,-25} | {2,-15}", "ID", "Назва статті", "Автор");
                    Console.WriteLine(new string('-', 50));
                    foreach (var a in articles)
                    {
                        Console.WriteLine("{0,-5} | {1,-25} | {2,-15}", a.Id, a.Title, a.AuthorName);
                    }
                }

                Console.WriteLine("\n[ID] - Читати/Коментувати | [0] - Назад");
                Console.Write("\nВаш вибір: ");

                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    if (id == 0) break;

                    var selected = articles.FirstOrDefault(x => x.Id == id);
                    if (selected != null)
                    {
                        ShowArticleDetails(service, user, selected);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        MenuHelper.ShowError("Статтю з таким ID не знайдено.");
                        Console.ResetColor();
                    }
                }
            }
        }

        public static void ShowArticleDetails(BlogService service, UserDTO? user, ArticleDTO article)
        {
            bool continueViewing = true;
            while (continueViewing)
            {
                ArticleUI.RenderContent(article, user);

                var allComments = service.GetCommentsByArticle(article.Id).ToList();
                var indexMap = CommentUI.RenderComments(allComments);

                CommentUI.RenderNavigationHint(user, allComments);

                Console.Write("\nВаш вибір: ");
                string input = Console.ReadLine()?.ToUpper();

                continueViewing = ContentActionHandler.Handle(input, service, user, article, indexMap);
            }
        } 
    }
}