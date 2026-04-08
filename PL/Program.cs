using BLL.DTO;
using BLL.Services;
using PL.Content;
using PL.Menu;

namespace PL
{
    class Program
    {
        static AuthService _authService = new AuthService();
        static BlogService _blogService = new BlogService();
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
                        _currentUser = AuthFlow.Register(_authService);
                        if (_currentUser != null) 
                            MainMenu.Show(_blogService, _currentUser);
                        _currentUser = null; 
                    }
                    else if (choice == "2")
                    {
                        _currentUser = AuthFlow.Login(_authService);
                        if (_currentUser != null)
                            MainMenu.Show(_blogService, _currentUser);
                        _currentUser = null;
                    }
                    else if (choice == "3")
                    {
                        MainMenu.Show(_blogService, null);
                    }
                    else if (choice == "0")
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