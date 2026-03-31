using BLL.Services;
using BLL.DTO;

namespace PL
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

                // Показуємо ці пункти тільки якщо це НЕ гість
                if (user != null)
                {
                    Console.WriteLine("3. Створення статті");
                    Console.WriteLine("4. Створення рубрики");
                }

                Console.WriteLine("0. " + (user == null ? "Повернутися до входу" : "Вийти з акаунту"));
                Console.Write("\nОберіть дію: ");

                var choice = Console.ReadLine();

                if (choice == "1") ContentManager.ShowArticlesMenu(service, user);
                else if (choice == "2") ShowCategories(service);
                else if (choice == "3" && user != null) CreateArticleFlow(service, user);
                else if (choice == "4" && user != null) CreateCategoryFlow(service);
                else if (choice == "0") break;
                else MenuHelper.ShowError("Ця дія недоступна для гостей або невірна команда.");
            }
        }

        static void CreateArticleFlow(BlogService service, UserDTO user)
        {

            MenuHelper.ShowHeader("Нова стаття", user.Nickname);
            Console.Write("Заголовок: "); string t = Console.ReadLine();
            Console.Write("Зміст: "); string c = Console.ReadLine();
            service.CreateArticle(user.Id, t, c);
            MenuHelper.ShowSuccess("Статтю опубліковано!");
        }

        static void ShowCategories(BlogService service)
        {

            MenuHelper.ShowHeader("Рубрики");
            var cats = service.GetCategories();
            foreach (var c in cats) Console.WriteLine($"- {c.Name}");
            MenuHelper.Wait();
        }

        static void CreateCategoryFlow(BlogService service)
        {

            Console.Write("Назва нової рубрики: ");
            service.CreateCategory(Console.ReadLine());
            MenuHelper.ShowSuccess("Рубрику додано!");
        }
    }
}