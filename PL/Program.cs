using BLL.DTO;
using BLL.Services;

namespace PL
{
    class Program
    {
        static BlogService _service = new BlogService();
        static UserDTO? _currentUser = null;

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                MenuHelper.ShowHeader("Система Блогів | Вхід");
                Console.WriteLine("1. Реєстрація нових користувачів");
                Console.WriteLine("2. Вхід в особистий кабінет");
                Console.WriteLine("3. Продовжити як гість (Тільки перегляд)");
                Console.WriteLine("0. Вийти з програми");
                Console.Write("\nВаш вибір: ");

                var choice = Console.ReadLine();
                try
                {
                    if (choice == "1")
                    {
                        _currentUser = AuthFlow.Register(_service);
                        if (_currentUser != null) MainMenu.Show(_service, _currentUser);
                        _currentUser = null; // ДОДАТИ ЦЕ: щоб при виході з MainMenu акаунт "скидався"
                    }
                    else if (choice == "2")
                    {
                        _currentUser = AuthFlow.Login(_service);
                        if (_currentUser != null) MainMenu.Show(_service, _currentUser);
                        _currentUser = null; // ДОДАТИ ЦЕ
                    }
                    else if (choice == "3")
                    {
                        // ВХІД ЯК ГІСТЬ: передаємо null замість користувача
                        MainMenu.Show(_service, null);
                    }
                    else if (choice == "0")
                        break;
                }
                catch (Exception ex) { MenuHelper.ShowError(ex.Message); }
            }
        }
    }
}