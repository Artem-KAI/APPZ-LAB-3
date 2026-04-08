using BLL.Services;
using BLL.DTO;
using PL.Menu;

namespace PL.Content
{
    public static class AuthFlow
    {
        public static UserDTO Register(BlogService service)
        {
            MenuHelper.ShowHeader("Реєстрація");

            string n, e, p1, p2;

            while (true)
            {
                Console.Write("Nickname: ");
                n = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(n) && n.Length >= 3) 
                    break;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Помилка: Нікнейм має бути не менше 3 символів.");
                Console.ResetColor();
            }

            while (true)
            {
                Console.Write("Email: ");
                e = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(e) && e.Contains("@") && e.Contains(".")) 
                    break;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Помилка: Введіть коректний Email.");
                Console.ResetColor();
            }

            while (true)
            {
                Console.Write("Пароль: ");
                p1 = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(p1) && p1.Length >= 6) 
                    break;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Помилка: Пароль має бути не менше 6 символів.");
                Console.ResetColor();
            }

            while (true)
            {
                Console.Write("Повтор пароля: ");
                p2 = Console.ReadLine();

                if (p1 == p2) 
                    break;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Помилка: Паролі не збігаються!");
                Console.ResetColor();
            }

            try
            {         
                var user = service.Register(n, e, p1, p2);
                MenuHelper.ShowSuccess("Акаунт створено!");
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка реєстрації: {ex.Message}");
                return null;
            }
        }

        public static UserDTO Login(BlogService service)
        {
            MenuHelper.ShowHeader("Вхід");

            Console.Write("Email: "); 
            string e = Console.ReadLine();

            Console.Write("Пароль: "); 
            string p = Console.ReadLine();

            var user = service.Login(e, p);
            if (user == null)
            {
                throw new Exception("Невірний Email або пароль!");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            MenuHelper.ShowSuccess($"Вітаємо, {user.Nickname}!");
            Console.ResetColor();
            return user;
        }
    }
}