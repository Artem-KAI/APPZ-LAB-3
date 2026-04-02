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
            Console.WriteLine($"Автор: {article.AuthorName}\n" + new string('=', 40));
            Console.WriteLine($"{article.Content}\n" + new string('=', 40));

            // Кнопка видалення статті для автора
            bool isArticleAuthor = user != null && article.AuthorName == user.Nickname;
            if (isArticleAuthor)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[D] - ВИДАЛИТИ ЦЮ СТАТТЮ");
                Console.ResetColor();
            }

            Console.WriteLine("\nКОМЕНТАРІ:");
            var allComments = service.GetCommentsByArticle(article.Id).ToList();

            // Словник для швидкого пошуку ID бази за "красивим" індексом (наприклад, "2.1" -> ID: 42)
            var indexMap = new Dictionary<string, int>();

            if (!allComments.Any()) Console.WriteLine("Коментарів ще немає.");
            else
            {
                var rootComments = allComments.Where(c => c.ParentId == null).ToList();
                for (int i = 0; i < rootComments.Count; i++)
                {
                    var c = rootComments[i];
                    string rootIndex = (i + 1).ToString(); // [1], [2]...
                    indexMap[rootIndex] = c.Id;

                    Console.WriteLine($"> [{rootIndex}] {c.AuthorName}: {c.Text}");

                    var replies = allComments.Where(r => r.ParentId == c.Id).ToList();
                    for (int j = 0; j < replies.Count; j++)
                    {
                        var r = replies[j];
                        string replyIndex = $"{rootIndex}.{j + 1}"; // [1.1], [1.2]...
                        indexMap[replyIndex] = r.Id;

                        Console.WriteLine($"  └── [Reply to {c.AuthorName}] [{replyIndex}] {r.AuthorName}: {r.Text}");
                    }
                }
            }

            Console.WriteLine("\n[1] Коментувати | [R] Відповісти | [X] Видалити коментар | [0] Назад");
            Console.Write("\nВаш вибір: ");
            string input = Console.ReadLine()?.ToUpper();

            // 1. ВІДПОВІДЬ (за новим індексом)
            if (input == "R" && user != null)
            {
                Console.Write("Введіть номер коментаря (напр. 1 або 2.1): ");
                string idx = Console.ReadLine();
                if (indexMap.ContainsKey(idx))
                {
                    Console.Write($"Ваша відповідь на [{idx}]: ");
                    string txt = Console.ReadLine();
                    service.ReplyToComment(user.Id, article.Id, indexMap[idx], txt);
                    MenuHelper.ShowSuccess("Відповідь додано!");
                    ShowArticleDetails(service, user, article);
                }
                else MenuHelper.ShowError("Невірний номер коментаря!");
            }
            // 2. ВИДАЛЕННЯ (за новим індексом)
            else if (input == "X" && user != null)
            {
                Console.Write("Введіть номер вашого коментаря для видалення (напр. 2.3): ");
                string idx = Console.ReadLine();
                if (indexMap.ContainsKey(idx))
                {
                    try
                    {
                        service.DeleteComment(user.Id, indexMap[idx]);
                        MenuHelper.ShowSuccess("Видалено!");
                        ShowArticleDetails(service, user, article);
                    }
                    catch (Exception ex) { MenuHelper.ShowError(ex.Message); }
                }
                else MenuHelper.ShowError("Коментар не знайдено!");
            }
            // 3. НОВИЙ КОРЕНЕВИЙ КОМЕНТАР
            else if (input == "1" && user != null)
            {
                Console.Write("Ваш коментар: ");
                service.CreateComment(user.Id, article.Id, Console.ReadLine());
                ShowArticleDetails(service, user, article);
            }
            // 4. ВИДАЛЕННЯ СТАТТІ
            else if (input == "D" && isArticleAuthor)
            {
                Console.Write("Видалити статтю? (y/n): ");
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    service.DeleteArticle(user.Id, article.Id);
                    return;
                }
            }
        }

        //private static void ShowArticleDetails(BlogService service, UserDTO user, ArticleDTO article)
        //{
        //    MenuHelper.ShowHeader(article.Title, user?.Nickname);
        //    Console.WriteLine($"Автор: {article.AuthorName}");
        //    Console.WriteLine(new string('=', 40));
        //    Console.WriteLine($"\n{article.Content}\n");
        //    Console.WriteLine(new string('=', 40));

        //    // Перевірка на автора статті (для видалення)
        //    bool isArticleAuthor = user != null && article.AuthorName == user.Nickname;
        //    if (isArticleAuthor)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Red;
        //        Console.WriteLine("[D] - ВИДАЛИТИ ЦЮ СТАТТЮ");
        //        Console.ResetColor();
        //    }

        //    Console.WriteLine("\nКОМЕНТАРІ:");
        //    var allComments = service.GetCommentsByArticle(article.Id).ToList();

        //    if (!allComments.Any())
        //    {
        //        Console.WriteLine("Коментарів ще немає.");
        //    }
        //    else
        //    {
        //        // 1. Спершу виводимо головні коментарі (у яких ParentId == null)
        //        var rootComments = allComments.Where(c => c.ParentId == null);
        //        foreach (var c in rootComments)
        //        {
        //            Console.WriteLine($"> [{c.Id}] {c.AuthorName}: {c.Text}");

        //            // 2. Виводимо відповіді до цього коментаря з відступом
        //            var replies = allComments.Where(r => r.ParentId == c.Id);
        //            foreach (var r in replies)
        //            {
        //                Console.WriteLine($"    └── [{r.Id}] {r.AuthorName}: {r.Text}");
        //            }
        //        }
        //    }

        //    // МЕНЮ ДІЙ
        //    Console.WriteLine("\n[1] Коментувати | [R] Відповісти | [0] Назад");

        //    if (user != null && allComments.Any(c => c.AuthorName == user.Nickname))
        //        Console.WriteLine("[X] Видалити свій коментар");

        //    Console.Write("\nВаш вибір: ");
        //    string input = Console.ReadLine()?.ToUpper();

        //    // ЛОГІКА ВІДПОВІДІ (REPLY)
        //    if (input == "R" && user != null)
        //    {
        //        Console.Write("Введіть ID коментаря, на який відповідаєте: ");
        //        if (int.TryParse(Console.ReadLine(), out int pId))
        //        {
        //            Console.Write("Ваша відповідь: ");
        //            string replyText = Console.ReadLine();
        //            if (!string.IsNullOrWhiteSpace(replyText))
        //            {
        //                service.ReplyToComment(user.Id, article.Id, pId, replyText);
        //                MenuHelper.ShowSuccess("Відповідь додано!");
        //                ShowArticleDetails(service, user, article); // Рекурсивне оновлення
        //            }
        //        }
        //    }
        //    // ЛОГІКА ВИДАЛЕННЯ СТАТТІ
        //    else if (input == "D" && isArticleAuthor)
        //    {
        //        Console.Write("Видалити статтю назавжди? (y/n): ");
        //        if (Console.ReadLine()?.ToLower() == "y")
        //        {
        //            service.DeleteArticle(user.Id, article.Id);
        //            MenuHelper.ShowSuccess("Статтю видалено.");
        //            return;
        //        }
        //    }
        //    // ЛОГІКА ВИДАЛЕННЯ КОМЕНТАРЯ
        //    else if (input == "X" && user != null)
        //    {
        //        Console.Write("ID вашого коментаря для видалення: ");
        //        if (int.TryParse(Console.ReadLine(), out int commId))
        //        {
        //            try
        //            {
        //                service.DeleteComment(user.Id, commId);
        //                MenuHelper.ShowSuccess("Видалено.");
        //                ShowArticleDetails(service, user, article);
        //            }
        //            catch (Exception ex) { MenuHelper.ShowError(ex.Message); }
        //        }
        //    }
        //    // ДОДАВАННЯ ЗВИЧАЙНОГО КОМЕНТАРЯ
        //    else if (input == "1" && user != null)
        //    {
        //        Console.Write("Текст коментаря: ");
        //        string text = Console.ReadLine();
        //        if (!string.IsNullOrWhiteSpace(text))
        //        {
        //            service.CreateComment(user.Id, article.Id, text);
        //            MenuHelper.ShowSuccess("Додано!");
        //            ShowArticleDetails(service, user, article);
        //        }
        //    }
        //}

        //private static void ShowArticleDetails(BlogService service, UserDTO user, ArticleDTO article)
        //{
        //    MenuHelper.ShowHeader(article.Title, user?.Nickname);
        //    Console.WriteLine($"Автор: {article.AuthorName}");
        //    Console.WriteLine(new string('=', 40));
        //    Console.WriteLine($"\n{article.Content}\n");
        //    Console.WriteLine(new string('=', 40));

        //    // Перевірка на автора статті
        //    bool isArticleAuthor = user != null && article.AuthorName == user.Nickname;

        //    if (isArticleAuthor)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Red;
        //        Console.WriteLine("[D] - ВИДАЛИТИ ЦЮ СТАТТЮ");
        //        Console.ResetColor();
        //    }

        //    Console.WriteLine("\nКОМЕНТАРІ:");
        //    var comments = service.GetCommentsByArticle(article.Id).ToList();

        //    if (!comments.Any()) Console.WriteLine("Коментарів ще немає.");
        //    else
        //    {
        //        foreach (var c in comments)
        //        {
        //            // Якщо юзер автор коментаря - показуємо ID для видалення
        //            string deleteHint = (user != null && c.AuthorName == user.Nickname)
        //                ? $" (Ваш коментар, ID для видалення: {c.Id})"
        //                : "";
        //            Console.WriteLine($"> {c.AuthorName}: {c.Text}{deleteHint}");
        //        }
        //    }

        //    Console.WriteLine("\n[1] Коментувати | [0] Назад");
        //    if (user != null && comments.Any(c => c.AuthorName == user.Nickname))
        //    {
        //        Console.WriteLine("[X] Видалити свій коментар");
        //    }

        //    Console.Write("\nВаш вибір: ");
        //    string input = Console.ReadLine()?.ToUpper();

        //    // ЛОГІКА ВИДАЛЕННЯ СТАТТІ
        //    if (input == "D" && isArticleAuthor)
        //    {
        //        Console.Write("Видалити статтю назавжди? (y/n): ");
        //        if (Console.ReadLine()?.ToLower() == "y")
        //        {
        //            service.DeleteArticle(user.Id, article.Id);
        //            MenuHelper.ShowSuccess("Статтю видалено.");
        //            return;
        //        }
        //    }
        //    // ЛОГІКА ВИДАЛЕННЯ КОМЕНТАРЯ
        //    else if (input == "X" && user != null)
        //    {
        //        Console.Write("Введіть ID вашого коментаря для видалення: ");
        //        if (int.TryParse(Console.ReadLine(), out int commId))
        //        {
        //            try
        //            {
        //                service.DeleteComment(user.Id, commId);
        //                MenuHelper.ShowSuccess("Коментар видалено.");
        //                ShowArticleDetails(service, user, article); // Оновлюємо сторінку
        //            }
        //            catch (Exception ex) { MenuHelper.ShowError(ex.Message); }
        //        }
        //    }
        //    // ДОДАВАННЯ КОМЕНТАРЯ
        //    else if (input == "1" && user != null)
        //    {
        //        Console.Write("Текст коментаря: ");
        //        string text = Console.ReadLine();
        //        if (!string.IsNullOrWhiteSpace(text))
        //        {
        //            service.CreateComment(user.Id, article.Id, text);
        //            MenuHelper.ShowSuccess("Додано!");
        //            ShowArticleDetails(service, user, article);
        //        }
        //    }
        //}

        //private static void ShowArticleDetails(BlogService service, UserDTO user, ArticleDTO article)
        //{
        //    MenuHelper.ShowHeader(article.Title, user?.Nickname);
        //    Console.WriteLine($"Автор: {article.AuthorName}");
        //    Console.WriteLine(new string('=', 40));
        //    Console.WriteLine($"\n{article.Content}\n");
        //    Console.WriteLine(new string('=', 40));

        //    // Перевірка на автора статті
        //    bool isArticleAuthor = user != null && article.AuthorName == user.Nickname;

        //    if (isArticleAuthor)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Red;
        //        Console.WriteLine("[D] - ВИДАЛИТИ ЦЮ СТАТТЮ");
        //        Console.ResetColor();
        //    }

        //    Console.WriteLine("\nКОМЕНТАРІ:");
        //    var comments = service.GetCommentsByArticle(article.Id).ToList();

        //    if (!comments.Any()) Console.WriteLine("Коментарів ще немає.");
        //    else
        //    {
        //        foreach (var c in comments)
        //        {
        //            // Якщо юзер автор коментаря - показуємо ID для видалення
        //            string deleteHint = (user != null && c.AuthorName == user.Nickname)
        //                ? $" (Ваш коментар, ID для видалення: {c.Id})"
        //                : "";
        //            Console.WriteLine($"> {c.AuthorName}: {c.Text}{deleteHint}");
        //        }
        //    }

        //    Console.WriteLine("\n[1] Коментувати | [0] Назад");
        //    if (user != null && comments.Any(c => c.AuthorName == user.Nickname))
        //    {
        //        Console.WriteLine("[X] Видалити свій коментар");
        //    }

        //    Console.Write("\nВаш вибір: ");
        //    string input = Console.ReadLine()?.ToUpper();

        //    // ЛОГІКА ВИДАЛЕННЯ СТАТТІ
        //    if (input == "D" && isArticleAuthor)
        //    {
        //        Console.Write("Видалити статтю назавжди? (y/n): ");
        //        if (Console.ReadLine()?.ToLower() == "y")
        //        {
        //            service.DeleteArticle(user.Id, article.Id);
        //            MenuHelper.ShowSuccess("Статтю видалено.");
        //            return;
        //        }
        //    }
        //    // ЛОГІКА ВИДАЛЕННЯ КОМЕНТАРЯ
        //    else if (input == "X" && user != null)
        //    {
        //        Console.Write("Введіть ID вашого коментаря для видалення: ");
        //        if (int.TryParse(Console.ReadLine(), out int commId))
        //        {
        //            try
        //            {
        //                service.DeleteComment(user.Id, commId);
        //                MenuHelper.ShowSuccess("Коментар видалено.");
        //                ShowArticleDetails(service, user, article); // Оновлюємо сторінку
        //            }
        //            catch (Exception ex) { MenuHelper.ShowError(ex.Message); }
        //        }
        //    }
        //    // ДОДАВАННЯ КОМЕНТАРЯ
        //    else if (input == "1" && user != null)
        //    {
        //        Console.Write("Текст коментаря: ");
        //        string text = Console.ReadLine();
        //        if (!string.IsNullOrWhiteSpace(text))
        //        {
        //            service.CreateComment(user.Id, article.Id, text);
        //            MenuHelper.ShowSuccess("Додано!");
        //            ShowArticleDetails(service, user, article);
        //        }
        //    }
        //}
    }
}