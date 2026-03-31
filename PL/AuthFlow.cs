using BLL.Services;
using BLL.DTO;

namespace PL
{
    public static class AuthFlow
    {
        public static UserDTO Register(BlogService service)
        {
            MenuHelper.ShowHeader("Реєстрація");
            Console.Write("Nickname: "); string n = Console.ReadLine();
            Console.Write("Email: "); string e = Console.ReadLine();
            Console.Write("Пароль: "); string p1 = Console.ReadLine();
            Console.Write("Повтор пароля: "); string p2 = Console.ReadLine();

            var user = service.Register(n, e, p1, p2);
            MenuHelper.ShowSuccess("Акаунт створено!");
            return user;
        }

        public static UserDTO Login(BlogService service)
        {
            MenuHelper.ShowHeader("Вхід");
            Console.Write("Email: "); string e = Console.ReadLine();
            Console.Write("Пароль: "); string p = Console.ReadLine();

            var user = service.Login(e, p);
            if (user == null) throw new Exception("Невірний Email або пароль!");

            MenuHelper.ShowSuccess($"Вітаємо, {user.Nickname}!");
            return user;
        }
    }
}