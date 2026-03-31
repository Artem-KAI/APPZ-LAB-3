using BLL.Services;
using BLL.DTO;

namespace PL
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
                        MenuHelper.ShowError("Статтю з таким ID не знайдено.");
                    }
                }
            }
        }

        private static void ShowArticleDetails(BlogService service, UserDTO user, ArticleDTO article)
        {
            MenuHelper.ShowHeader(article.Title, user?.Nickname);
            Console.WriteLine($"Автор: {article.AuthorName}");
            Console.WriteLine(new string('=', 40));
            Console.WriteLine($"\n{article.Content}\n");
            Console.WriteLine(new string('=', 40));

            // --- ВИВІД КОМЕНТАРІВ ---
            Console.WriteLine("\nКОМЕНТАРІ:");
            var comments = service.GetCommentsByArticle(article.Id);

            if (!comments.Any())
            {
                Console.WriteLine("Поки що немає жодного коментаря. Будьте першим!");
            }
            else
            {
                foreach (var c in comments)
                {
                    Console.WriteLine($"> [{c.AuthorName}]: {c.Text}");
                }
            }
            Console.WriteLine(new string('-', 40));

            // Логіка додавання нового коментаря
            if (user != null)
            {
                Console.WriteLine("\n[1] Залишити коментар | [0] Назад");
                if (Console.ReadLine() == "1")
                {
                    Console.Write("Ваш коментар: ");
                    string text = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        service.CreateComment(user.Id, article.Id, text);
                        MenuHelper.ShowSuccess("Ваш голос почуто!");
                        // Рекурсивно викликаємо цей же метод, щоб оновити список коментарів
                        ShowArticleDetails(service, user, article);
                    }
                }
            }
            else
            {
                Console.WriteLine("\n(Увійдіть в акаунт, щоб коментувати)");
                MenuHelper.Wait();
            }
        }
    }
}