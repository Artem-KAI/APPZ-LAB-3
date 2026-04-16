using BLL.Services;
using BLL.DTO;
using PL.Content;

namespace PL.Menu
{
    public static class MainMenu
    {
        public static void Show(BlogService service, UserDTO? user)
        {
            while (true)
            {
                string name = user?.Nickname ?? "Гість";
                MenuHelper.ShowHeader("Робочий стіл", name);

                Console.WriteLine("1. Перегляд статей");
                Console.WriteLine("2. Перегляд рубрик");

                if (user != null)
                {
                    Console.WriteLine("3. Створення статті");
                    Console.WriteLine("4. Створення рубрики");
                }

                Console.WriteLine("0. " + (user == null ? "Повернутися до входу" : "Вийти з акаунту"));
                Console.Write("\nОберіть дію: ");

                var choice = Console.ReadLine();

                if (choice == "1")
                    ContentManager.ShowArticlesMenu(service, user);
                else if (choice == "2")
                    ShowCategories(service, user);
                else if (choice == "3" && user != null)
                    CreateArticleFlow(service, user);
                else if (choice == "4" && user != null)
                    CreateCategoryFlow(service);
                else if (choice == "0")
                    break;
                else
                {   
                    Console.ForegroundColor = ConsoleColor.Red;
                    MenuHelper.ShowError("Ця дія недоступна для гостей або невірна команда.");
                    Console.ResetColor();
                }
            }
        }

        static void CreateArticleFlow(BlogService service, UserDTO user)
        {
            MenuHelper.ShowHeader("Нова стаття", user.Nickname);

            var categories = service.GetCategories().ToList();
            if (!categories.Any())
            {
                MenuHelper.ShowError("Немає доступних рубрик. Спочатку створіть рубрику!");
                return;
            }

            Console.WriteLine("Оберіть рубрику:");
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i].Name}");
            }

            Console.Write("\nВаш вибір (номер): ");
            if (int.TryParse(Console.ReadLine(), out int catIdx) && catIdx > 0 && catIdx <= categories.Count)
            {
                var selectedCat = categories[catIdx - 1];

                Console.Write("Заголовок: ");
                string t = Console.ReadLine();

                Console.Write("Зміст: "); 
                string c = Console.ReadLine();

                service.CreateArticle(user.Id, selectedCat.Id, t, c);

                MenuHelper.ShowSuccess($"Статтю опубліковано в рубриці '{selectedCat.Name}'!");
            }
            else
            {
                MenuHelper.ShowError("Невірний вибір рубрики.");
            }
        }

        static void ShowCategories(BlogService service, UserDTO? user)
        {
            while (true)
            {
                MenuHelper.ShowHeader("Рубрики", user?.Nickname ?? "Гість");
                var cats = service.GetCategories().ToList();

                if (!cats.Any())
                {
                    Console.WriteLine("- Рубрик поки немає");
                    MenuHelper.Wait();
                    break;
                }

                for (int i = 0; i < cats.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {cats[i].Name}");
                }

                Console.WriteLine("\n[ID] - Переглянути статті");

                if (user != null)
                {
                    Console.WriteLine("[D + ID] - Видалити рубрику (наприклад: d1)");
                }

                Console.WriteLine("[0] - Назад");
                Console.Write("\nВаш вибір: ");

                string input = Console.ReadLine()?.Trim().ToLower() ?? "";

                if (input == "0") 
                    break;

                if (input.StartsWith("d") && user != null)
                {
                    if (int.TryParse(input.Substring(1), out int deleteIndex) && deleteIndex > 0 && deleteIndex <= cats.Count)
                    {
                        var catToDelete = cats[deleteIndex - 1];
                        try
                        {
                            service.DeleteCategory(catToDelete.Id);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"\nРубрику '{catToDelete.Name}' успішно видалено!");
                            Console.ResetColor();
                            MenuHelper.Wait();
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            MenuHelper.ShowError(ex.Message);
                            Console.ResetColor();
                        } 
                    }
                    else
                    {
                        MenuHelper.ShowError("Невірний формат. Введіть d та номер (наприклад, d1).");
                    }
                        
                    continue; 
                }

                if (int.TryParse(input, out int choice))
                {
                    if (choice > 0 && choice <= cats.Count)
                    {
                        ShowArticlesByCategory(service, user, cats[choice - 1]);
                    }
                }
            }
        }

        static void ShowArticlesByCategory(BlogService service, UserDTO? user, CategoryDTO cat)
        {
            while (true)
            {
                MenuHelper.ShowHeader($"Статті: {cat.Name}", user?.Nickname ?? "Гість");

                var articles = service.GetArticles().Where(a => a.CategoryId == cat.Id).ToList();

                if (!articles.Any())
                {
                    Console.WriteLine("У цій рубриці ще немає статтей.");
                    MenuHelper.Wait();
                    break;
                }

                Console.WriteLine("{0,-5} | {1,-25} | {2,-15}", "ID", "Назва", "Автор");
                Console.WriteLine(new string('-', 50));
                foreach (var a in articles)
                {
                    Console.WriteLine("{0,-5} | {1,-25} | {2,-15}", a.Id, a.Title, a.AuthorName);
                }

                Console.WriteLine("\n[ID] - Читати статтю | [0] - Назад");
                Console.Write("\nВаш вибір: ");

                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    if (id == 0) 
                        break;

                    var selected = articles.FirstOrDefault(x => x.Id == id);
                    if (selected != null)
                    {
                        PL.Content.ContentManager.ShowArticleDetails(service, user, selected);
                    }
                    else
                    {
                        MenuHelper.ShowError("Статтю не знайдено.");
                    }
                }
            }
        }

        static void CreateCategoryFlow(BlogService service)
        {
            while (true) 
            {
                try
                {
                    Console.Write("Назва нової рубрики (або 0 для скасування): ");
                    string name = Console.ReadLine();

                    if (name == "0") 
                        break; 

                    service.CreateCategory(name);

                    MenuHelper.ShowSuccess("Рубрику додано!");
                    break;
                }
                catch (Exception ex)
                {
                    MenuHelper.ShowError(ex.Message);
                }
            }
        }
    }
}